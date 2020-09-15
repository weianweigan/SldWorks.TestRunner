using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using CADApplication.TestRunner.Runner.Direct;
using CADApplication.TestRunner.Runner.NUnit;
using CADApplication.TestRunner.View.TestTreeView;

namespace CADApplication.TestRunner.View
{
    /// <summary>
    /// ViewModel for TestRunner
    /// </summary>
    /// <typeparam name="TApp"></typeparam>
    public class NUnitRunnerViewModel<TApp> : DialogViewModel
    {
        #region Fields, Constructor

        private TApp mUiApplication;
        private string mAssemblyPath;
        private string mProgramState;

        public NUnitRunnerViewModel(TApp uiApplication )
        {
            InitialHeight = 500;
            InitialWidth = 800;
            DisplayName = "Test Runner";

            mUiApplication = uiApplication;

            Tree = new TreeViewModel();
            Tree.PropertyChanged += ( o, args ) => OnPropertyChangedAll();

            //求解程序集
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            if ( !string.IsNullOrEmpty( Properties.Settings.Default.AssemblyPath ) ) {
                LoadAssembly( Properties.Settings.Default.AssemblyPath );
            }
        }

        #endregion

        #region Properties

        /// <summary>程序集版本</summary>
        public string ProgramVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>TreeView Data</summary>
        public TreeViewModel Tree { get; set; }

        /// <summary>Filepath of the test assembly.</summary>
        public string AssemblyPath
        {
            get => mAssemblyPath;
            set
            {
                if( value == mAssemblyPath ) return;
                mAssemblyPath = value;
                OnPropertyChanged( () => AssemblyPath );
            }
        }

        public string DetailInformation
        {
            get
            {
                string result = string.Empty;

                if( Tree.SelectedNode != null ) {
                    result = $"{Tree.SelectedNode.Message}\n{Tree.SelectedNode.StackTrace}";
                }

                return result;
            }
        }

        public TestState? State => Tree?.SelectedNode?.State;

        public string ProgramState
        {
            get => mProgramState;
            set
            {
                if( value == mProgramState ) return;
                mProgramState = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand OpenLogCommand => new DelegateWpfCommand( () => Process.Start( Log.LogFilePath ), () => File.Exists( Log.LogFilePath ) );

        public ICommand OpenLogFloderCommand => new DelegateWpfCommand(() => Process.Start(Log.LogDirectory), () => Directory.Exists(Log.LogDirectory));

        /// <summary>打开程序集</summary>
        public ICommand OpenAssemblyCommand => new DelegateWpfCommand( ExecuteOpenAssemblyCommand );

        #region Start Test Commands

        public ICommand DebugCommand => new DelegateWpfCommand(ExecuteWithReflection, () => Tree.SelectedNode != null);

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// 指定加载读取某个程序集
        /// </summary>
        /// <param name="dialog"></param>
        public void OpenAssembly(string assemblyPath)
        {
            Properties.Settings.Default.AssemblyPath = AssemblyPath;
            Properties.Settings.Default.Save();

            LoadAssembly(assemblyPath);
        }

        public void SetAppInstance(TApp app)
        {
            if (app != null)
            {
                mUiApplication = app;
            }
        }

        #endregion

        #region Methods

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyPath = string.Empty;
            var assemblyName = new AssemblyName(args.Name).Name + ".dll";

            try
            {
                assemblyPath = Path.Combine(AssemblyPath, assemblyName);
                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
                else
                {
                    Debug.Print($"Assembly Load Error{assemblyPath}");
                }

                if (File.Exists(Properties.Settings.Default.AssemblyPath))
                {
                    var assemblyDirectory = Path.GetDirectoryName(Properties.Settings.Default.AssemblyPath);

                    assemblyPath = Path.Combine(assemblyDirectory, assemblyName);
                }

                return (File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("The location of the assembly, {0} could not be resolved for loading.", assemblyName), ex);
            }
        }
        private void ExecuteOpenAssemblyCommand()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                OpenAssembly(dialog.FileName);
            }
        }

        /// <summary>根据路径载入程序集</summary>
        /// <param name="path"></param>
        private void LoadAssembly( string path )
        {
            if( !string.IsNullOrEmpty( path ) && File.Exists( path ) ) {
                Tree.Clear();

                NUnitRunner runner = new NUnitRunner( path );
                AssemblyPath = runner.TestAssembly;

                runner.ExploreAssembly();

                if( runner.ExploreRun != null ) {
                    NodeViewModel root = ToNodeTree( runner.ExploreRun );

                    Tree.AddRootObject( root, true );
                    runner.Dispose();

                    ProgramState = $"Assembly loaded '{runner.TestAssembly}'";
                }

                runner.Dispose();
            }
        }

        /// <summary>使用反射执行程序集</summary>
        private void ExecuteWithReflection()
        {
            //记录起始信息
            DateTime start = DateTime.Now;
            ProgramState = "Test Run started...";

            //重设测试状态
            Tree.SelectedNode.Descendents.ToList().ForEach( n => n.Reset() );
            Tree.SelectedNode.Reset();

            //整理需要测试的节点
            var toRun = CasesToRun( Tree.SelectedNode ).ToList();

            //初始化Runner类
            ReflectionRunner runner = new ReflectionRunner( AssemblyPath );

            //逐个运行
            for( int i = 0; i < toRun.Count; i++ ) {
                ProgramState = $"Run Test {i + 1} of {toRun.Count}";
                runner.RunTest( toRun[ i ], mUiApplication );
            }

            //显示结果
            PresentResults( toRun, start );
        }

        private IEnumerable<NodeViewModel> CasesToRun( NodeViewModel root )
        {
            var list = new List<NodeViewModel>( root.Descendents ) { root };
            var cases = list.Where( n => n.Type == TestType.Case ).ToList();

            if( root.Type == TestType.Case ) cases.Add( root );

            var toRun = cases.Distinct().ToList();
            return toRun;
        }

        private NodeViewModel ToNodeTree( NUnitTestRun run )
        {
            NodeViewModel root = new NodeViewModel( run );

            foreach( NUnitTestSuite testSuite in run.TestSuites ) {
                ToNode( root, testSuite );
            }

            return root;
        }

        private void ToNode( NodeViewModel parent, NUnitTestSuite testSuite )
        {
            NodeViewModel node = new NodeViewModel( testSuite );
            parent.Add( node );
            node.Parent = parent;

            foreach( var suite in testSuite.TestSuites ) {
                ToNode( node, suite );
            }

            foreach( var test in testSuite.TestCases ) {
                ToNode( node, test );
            }
        }

        private void PresentResults( IEnumerable<NodeViewModel> tests, DateTime start )
        {
            DateTime end = DateTime.Now;
            bool success = tests.All( n => n.State == TestState.Passed );

            var cases = tests.Where( t => t.Type == TestType.Case );
            int passed = cases.Count( t => t.State == TestState.Passed );
            int failed = cases.Count( t => t.State == TestState.Failed );
            int unknown = cases.Count( t => t.State == TestState.Unknown );

            ProgramState = $"Test Run finished at {end:T}. Passed {passed} of {cases.Count()}";

            string message = $"Run finished at {end:T}\n\n" +
                             $"Passed Tests {passed} of {cases.Count()}\n" +
                             $"Run Duration {end - start}";


            MessageBox.Show( message,
                "TestRunner",
                MessageBoxButton.OK,
                success ? MessageBoxImage.Information : MessageBoxImage.Error );
        }
        #endregion
    }
}
