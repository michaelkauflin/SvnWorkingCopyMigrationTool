using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using SvnWorkingCopyMigrationTool.ViewModel;

namespace SvnWorkingCopyMigrationTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string DefaultFolder = @"c:\SVN";

        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void Button_Browse(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog
            {
                Title = "Select your working copy root folder",
                IsFolderPicker = true,
                InitialDirectory = DefaultFolder,
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                DefaultDirectory = DefaultFolder,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            };

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _viewModel.WorkingCopyRootPath = dlg.FileName;
            }
        }

        private void Button_Click_Migrate(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedWorkingCopy.Migrate();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            IEnumerable<WorkingCopyViewModel> wcvm = await _viewModel.Test();

            StringBuilder sb = new StringBuilder();
            wcvm.ToList().ForEach(w => sb.AppendLine(w.RootPath));
            MessageBox.Show(sb.ToString(), "Working Copies");
        }
    }
}
