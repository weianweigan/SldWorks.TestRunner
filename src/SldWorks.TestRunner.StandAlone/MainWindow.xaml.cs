using CADApplication.TestRunner.View;
using MahApps.Metro.Controls;
using SldWorks.TestRunner.StandAlone;
using SolidWorks.Interop.sldworks;
using System.Threading.Tasks;
using System.Windows;
using Xarial.XCad.SolidWorks;

namespace SldWorks.TestRunner.StandAlone
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow:MetroWindow
    {
        public MainWindowViewModel VM { get; }

        public MainWindow()
        {
            InitializeComponent();
            VM = new MainWindowViewModel(this);
            DataContext = VM;
        }
        
    }
}
