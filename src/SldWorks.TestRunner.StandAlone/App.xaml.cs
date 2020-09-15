using CADApplication.TestRunner.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CADApplication.TestRunner.StandAlone
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>使用命令行</summary>
        /// <remarks>
        /// *.exe test -file FilePath
        /// *.exe test -folder FolderPath
        /// -options addin
        /// </remarks>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if(e.Args != null)
            {
                
            }
        }

    }
}
