using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SvnWorkingCopyMigrationTool.Model;

namespace SvnWorkingCopyMigrationTool.ViewModel
{
    class MainViewModel : BaseViewModel
    {
        private WorkingCopyViewModel _selectedWorkingCopy;
        private readonly WorkingCopyFinder _finder = new WorkingCopyFinder();
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

        public async Task<IEnumerable<WorkingCopyViewModel>>  Test()
        {
            IEnumerable<WorkingCopy> workingCopies = await _finder.FindAll();

            return workingCopies.Select(wc => new WorkingCopyViewModel(wc));
        }
    }
}
