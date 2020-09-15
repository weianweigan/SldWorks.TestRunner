using CADApplication.TestRunner.View;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Xarial.XCad.SolidWorks;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SldWorks.TestRunner.StandAlone
{
    public class MainWindowViewModel : AbstractViewModel
    {
        #region Fields

        private NUnitRunnerViewModel<ISldWorks> _runnerViewModel;
        private ObservableCollection<NUnitRunnerViewModel<ISldWorks>> _files = new ObservableCollection<NUnitRunnerViewModel<ISldWorks>>();
        private NUnitRunnerViewModel<ISldWorks> _selectedFilePath;
        private readonly MainWindow _mainWindow;

        #endregion

        public MainWindowViewModel(MainWindow mainWindow)
        {
            this._mainWindow = mainWindow;
         
            GetSldWorksInstacneAsync();
        }

        #region Properties

        public bool FilterWithTest { get; set; } = true;

        public SwApplication App { get; private set; }

        /// <summary>测试资源管理器是否可见</summary>
        public bool TestRunnerVisiblity => SelectedRunner != null;

        public ObservableCollection<NUnitRunnerViewModel<ISldWorks>> Files { get => _files; }

        public string FolderPath
        {
            get => Properties.Settings.Default.AssemblyFolder; set
            {
                Properties.Settings.Default.AssemblyFolder = value;
                Properties.Settings.Default.Save();

                var files = Directory.GetFiles(value);
                var dlls = files.Where(p => (Path.GetExtension(p).ToLower() == ".dll") &&
                                      (FilterWithTest ? Path.GetFileNameWithoutExtension(p).EndsWith("Test") : true));
                foreach (var file in dlls)
                {
                    var viewModel = new NUnitRunnerViewModel<ISldWorks>(App?.Sw);
                    viewModel.OpenAssembly(file);
                    Files.Add(viewModel);
                }
            }
        }

        public NUnitRunnerViewModel<ISldWorks> SelectedRunner
        {
            get => _selectedFilePath; set
            {
                _selectedFilePath = value;
                OnPropertyChanged(() => SelectedRunner);
                OnPropertyChanged(() => TestRunnerVisiblity);
            }
        }

        public string SldWorksMsg => App == null ? "SolidWorks Instance Not Found" : $"{App.Version} Found";

        #endregion

        #region Commands

        public DelegateWpfCommand OpenFolderCommand => new DelegateWpfCommand(OpenFolderClick, () => true);

        public DelegateWpfCommand OpenFileCommand => new DelegateWpfCommand(OpenFileClick, () => true);

        public DelegateWpfCommand DeleteCommand => new DelegateWpfCommand(DeleteClick, CanDeleteClick);

        #endregion

        #region Private Methods

        private async void GetSldWorksInstacneAsync()
        {
            try
            {
                var process = Process.GetProcesses();

                var sldProcess = process.Where(p => Regex.IsMatch(p.ProcessName, "SLDWORKS")).FirstOrDefault();

                if (sldProcess == null)
                {
                    OnPropertyChanged(() => SldWorksMsg);
                    return;
                }

                App = SwApplication.FromProcess(sldProcess);
                
                if (App != null)
                {
                    SelectedRunner = new NUnitRunnerViewModel<ISldWorks>(App.Sw);
                }
                else
                {
                    //RunnerViewModel = new NUnitRunnerViewModel<ISldWorks>(null);
                }
            }
            catch (Exception ex)
            {
                _mainWindow.ShowMessageAsync("SolidWorks Instance Not Found", ex.Message,MessageDialogStyle.Affirmative);
            }
        }

        public void OpenFolderClick()
        {

            var folder = new System.Windows.Forms.FolderBrowserDialog();
            var result = folder.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FolderPath = folder.SelectedPath;
            }
        }

        private void DeleteClick()
        {
            Files.Remove(SelectedRunner);
        }

        private bool CanDeleteClick()
        {
            return (SelectedRunner != null);
        }

        private void OpenFileClick()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                var vm = new NUnitRunnerViewModel<ISldWorks>(App?.Sw);
                vm.OpenAssembly(dialog.FileName);
                Files.Add(vm);
            }
        }

        #endregion

    }
}
