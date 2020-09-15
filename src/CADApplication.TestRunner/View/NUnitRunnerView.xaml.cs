using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Controls;

namespace CADApplication.TestRunner.View
{
    /// <summary>
    /// Interaction logic for NUnitRunnerView.xaml
    /// </summary>
    public partial class NUnitRunnerView : UserControl
    {
        public NUnitRunnerView()
        {
            MahApps.Metro.Controls.ToggleSwitch tt = new MahApps.Metro.Controls.ToggleSwitch();
            //求解程序集
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            InitializeComponent();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Name))
            {
                return null;
            }
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

                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

                assemblyPath = Path.Combine(assemblyDirectory, assemblyName);
                return (File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("The location of the assembly, {0} could not be resolved for loading.", assemblyName), ex);
            }
        }

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
    }
}
