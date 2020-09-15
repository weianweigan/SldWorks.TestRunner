
/* 
  UnitTest Addin For SolidWorks.

 */

using CADApplication.TestRunner.View;
using SldWorks.TestRunner.Addin.Properties;
using SolidWorks.Interop.sldworks;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.Integration;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.UI.Commands.Attributes;

namespace SldWorks.TestRunner.Addin
{

    //[AutoRegister("TestRunner", "TestRunner for SolidWorks", true)]
    [ComVisible(true), Guid("AD273BD6-6BE5-4DC1-B153-9682DAD6ECE8")]
    public class TestRunnerAddin:SwAddInEx
    {
        #region Fields
        
        private ElementHost _host;
        private TaskpaneView _taskPane;
        private IDisposable m_SignalRHub;

        public const string URL = "http://localhost:8080/TestRunner/";

        #endregion

        #region .Ctor

        static TestRunnerAddin()
        {
            //AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        #endregion

        #region Properties

        public NUnitRunnerView RunnerView { get; private set; }

        public NUnitRunnerViewModel<ISldWorks> RunnerViewModel { get; private set; }

        public string AssemblyPath
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        #endregion

        #region Private Methods

        public override void OnConnect()
        {
            //添加按钮
            //AddCommandGroup<Commands_e>(OnCommandsButtonClick);

            //创建Taskpane
            CreateWPFTaskPane();

            //创建服务
            CreateService(URL);

        }

        public override void OnDisconnect()
        {
            // Dispose the add-in's resources
        }

        /// <summary>创建TaskPane</summary>
        private void CreateWPFTaskPane()
        {
            if (_taskPane == null)
            {
                _taskPane = base.Application.Sw.CreateTaskpaneView2($@"{AssemblyPath}\TestRunner32.png", "TestRunner");
            }

            if (RunnerView == null)
            {
                RunnerView = new NUnitRunnerView()
                {
                    DataContext = (RunnerViewModel  = new NUnitRunnerViewModel<ISldWorks>(base.Application.Sw))
                };
            }

            if (_host == null)
            {
                _host = new ElementHost()
                {
                    Child = RunnerView
                };
            }

            _taskPane.DisplayWindowFromHandlex64(_host.Handle.ToInt64());
        }

        private void OnCommandsButtonClick(Commands_e obj)
        {

        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var dir = Path.GetDirectoryName(typeof(TestRunnerAddin).Assembly.Location);
            var fileName = $"{new AssemblyName(args.Name).Name}.dll";
            return Assembly.LoadFile(Path.Combine(dir, fileName));
        }

        private void CreateService(string url)
        {
            
        }

        #endregion
    }

    #region Commands Config

    [Title(typeof(Resources), nameof(Resources.TestRunner))]
    [Description("TestRunner")]
    [Icon(typeof(Resources), nameof(Resources.TestRunner))]
    public enum Commands_e
    {
        [Title("TestRunner")]
        [Description("TestRunner for SolidWorks")]
        [Icon(typeof(Resources), nameof(Resources.TestRunner))]
        [CommandItemInfo(true, true,Xarial.XCad.UI.Commands.Enums.WorkspaceTypes_e.All,
    true,Xarial.XCad.UI.Commands.Enums.RibbonTabTextDisplay_e.TextBelow)]
        CommandTestRunner,
    }

    #endregion
}
