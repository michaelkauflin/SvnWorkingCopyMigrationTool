using SvnWorkingCopyMigrationTool.Model;

namespace SvnWorkingCopyMigrationTool.ViewModel
{
    class WorkingCopyViewModel: BaseViewModel
    {
        private WorkingCopy _workingCopy;
        private string _rootPath;
        private string _url;
        private string _repositoryRoot;
        private readonly WorkingCopyMigrationAnalyzer _analyzer;
        private string _migrationRequired;

        public string RootPath
        {
            get { return _rootPath; }
            private set
            {
                if (value == _rootPath) return;
                _rootPath = value;
                OnPropertyChanged();
            }
        }

        public string URL
        {
            get { return _url; }
            private set
            {
                if (value == _url) return;
                _url = value;
                OnPropertyChanged();
            }
        }

        public string RepositoryRoot
        {
            get { return _repositoryRoot; }
            private set
            {
                if (value == _repositoryRoot) return;
                _repositoryRoot = value;
                OnPropertyChanged();
            }
        }

        public string MigrationRequired
        {
            get { return _migrationRequired; }
            private set
            {
                if (value == _migrationRequired) return;
                _migrationRequired = value;
                OnPropertyChanged();
            }
        }

        public WorkingCopyViewModel(WorkingCopy workingCopy)
        {
            _analyzer = new WorkingCopyMigrationAnalyzer();
            Load(workingCopy);
        }

        public void Action()
        {
            _workingCopy.OpenRootFolder();
        }

        public void Refresh()
        {
            Load(WorkingCopy.Parse(_workingCopy.RootPath));
        }

        private void Load(WorkingCopy workingCopy)
        {
            _workingCopy = workingCopy;
            Dispatcher.Invoke(() =>
            {
                RootPath = workingCopy.RootPath.Replace("/", "\\");
                URL = workingCopy.URL;
                RepositoryRoot = workingCopy.RepositoryRoot;
                MigrationRequired = _analyzer.RequiresMigration(workingCopy)? "Yes" : "No";
            });   
        }
    }
}
