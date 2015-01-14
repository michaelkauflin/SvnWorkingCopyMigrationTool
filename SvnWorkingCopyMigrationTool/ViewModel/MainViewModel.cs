using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SvnWorkingCopyMigrationTool.Model;

namespace SvnWorkingCopyMigrationTool.ViewModel
{
    class MainViewModel : BaseViewModel
    {
        private readonly WorkingCopyFinder _finder = new WorkingCopyFinder();

        public ObservableCollection<WorkingCopyViewModel> WorkingCopies { get; private set; }
        public WorkingCopyViewModel SelectedWorkingCopyViewModel { get; set; }

        public MainViewModel()
        {
            WorkingCopies = new ObservableCollection<WorkingCopyViewModel>(); 
        }

        public async Task BrowseAutomatically()
        {
            IEnumerable<WorkingCopy> workingCopies = await _finder.FindAll();

            foreach (WorkingCopy workingCopy in workingCopies)
            {
                if (WorkingCopies.All(viewModel => viewModel.RootPath != workingCopy.RootPath))
                {
                    WorkingCopies.Add(new WorkingCopyViewModel(workingCopy));
                }
            }
        }

        public void Action()
        {
            if (SelectedWorkingCopyViewModel != null)
            {
                SelectedWorkingCopyViewModel.Action();
            }
        }
    }
}
