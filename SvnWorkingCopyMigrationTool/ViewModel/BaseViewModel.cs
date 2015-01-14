using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using SvnWorkingCopyMigrationTool.Annotations;

namespace SvnWorkingCopyMigrationTool.ViewModel
{
    /// <summary>
    /// Wrapper for property change notification, with the help of Resharper
    /// </summary>
    class BaseViewModel: INotifyPropertyChanged
    {
        protected readonly Dispatcher Dispatcher;

        public event PropertyChangedEventHandler PropertyChanged;

        public BaseViewModel()
        {
            Dispatcher = Application.Current.Dispatcher;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
