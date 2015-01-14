using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SvnWorkingCopyMigrationTool.Model;

namespace SvnWorkingCopyMigrationTool.ViewModel
{
    class MainViewModel : BaseViewModel
    {
        private WorkingCopyViewModel _selectedWorkingCopy;
        private string _workingCopyRootPath;

        public WorkingCopyViewModel SelectedWorkingCopy
        {
            get { return _selectedWorkingCopy; }
            private set
            {
                if (Equals(value, _selectedWorkingCopy)) return;
                _selectedWorkingCopy = value;
                OnPropertyChanged();
            }
        }

        public string WorkingCopyRootPath
        {
            get { return _workingCopyRootPath; }
            set
            {
                if (value == _workingCopyRootPath) return;
                _workingCopyRootPath = value;
                OnPropertyChanged();
                LoadWorkingCopy();
            }
        }

        private void LoadWorkingCopy()
        {
            Task.Run(() =>
            {
                if (Directory.Exists(WorkingCopyRootPath))
                {
                    SetWorkingCopy(WorkingCopyRootPath);
                }
            });
        }

        public void SetWorkingCopy(string rootFolderPath)
        {
            var newWorkingCopy = new WorkingCopyViewModel(WorkingCopy.Parse(rootFolderPath));
            Dispatcher.Invoke(() => SelectedWorkingCopy = newWorkingCopy);
        }

        public async Task Test()
        {
            
        }

        
    }
}
