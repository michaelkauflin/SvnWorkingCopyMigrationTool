﻿using System.Windows;
using System.Windows.Input;
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
            InitializeComponent();
            DataContext = _viewModel;
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

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            _viewModel.WorkingCopies.CollectionChanged += (o, args) =>
            {
                //MessageBox.Show(o.ToString(), args.Action.ToString());
            };
            await _viewModel.BrowseAutomatically();
            Cursor = Cursors.Arrow;
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _viewModel.Action();
        }
    }
}