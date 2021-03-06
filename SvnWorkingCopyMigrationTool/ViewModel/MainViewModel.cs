﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SvnWorkingCopyMigrationTool.Model;

namespace SvnWorkingCopyMigrationTool.ViewModel
{
    class MainViewModel : BaseViewModel
    {
        private readonly WorkingCopyFinder _finder = new WorkingCopyFinder();
        private string _browseRootFolder;
        private int _selectedBrowseDepth;

        public ObservableCollection<WorkingCopyViewModel> WorkingCopies { get; private set; }
        public WorkingCopyViewModel SelectedWorkingCopyViewModel { get; set; }

        public string BrowseRootFolder
        {
            get { return _browseRootFolder; }
            set
            {
                if (value == _browseRootFolder) return;
                _browseRootFolder = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> AvailableBrowseDepths { get; private set; }

        public int SelectedBrowseDepth
        {
            get { return _selectedBrowseDepth; }
            set
            {
                if (value == _selectedBrowseDepth) return;
                _selectedBrowseDepth = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            WorkingCopies = new ObservableCollection<WorkingCopyViewModel>();
            AvailableBrowseDepths = new ObservableCollection<int>(Enumerable.Range(1, 5));
            SelectedBrowseDepth = 2;
        }

        public async Task BrowseAutomatically()
        {
            bool foundNewWorkingCopies = false;

            IEnumerable<WorkingCopy> workingCopies;
            if (String.IsNullOrWhiteSpace(BrowseRootFolder))
            {
                workingCopies = await _finder.FindAll(SelectedBrowseDepth);
            }
            else if (!Directory.Exists(BrowseRootFolder))
            {
                MessageBox.Show(String.Format("Directory {0} does not exist", BrowseRootFolder), "Error");
                return;
            }
            else
            {
                workingCopies = _finder.FindInDirectory(BrowseRootFolder, SelectedBrowseDepth);
            }

            foreach (WorkingCopy workingCopy in workingCopies)
            {
                if (WorkingCopies.All(viewModel => viewModel.RootPath != workingCopy.RootPath.Replace("/", "\\")))
                {
                    foundNewWorkingCopies = true;
                    WorkingCopies.Add(new WorkingCopyViewModel(workingCopy));
                }
            }

            if (!foundNewWorkingCopies)
            {
                MessageBox.Show("Now new working copy found");
            }
        }

        public void Action()
        {
            if (SelectedWorkingCopyViewModel != null)
            {
                SelectedWorkingCopyViewModel.Action();
            }
        }

        public async Task Refresh()
        {
            await Task.Run(() =>
            {
                foreach (WorkingCopyViewModel vm in WorkingCopies)
                {
                    vm.Refresh();
                }
            });
        }

        public void Clear()
        {
            WorkingCopies.Clear();
        }
    }
}
