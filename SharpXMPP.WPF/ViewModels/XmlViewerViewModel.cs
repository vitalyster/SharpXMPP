using System.ComponentModel;
using System.Data.Entity;
using System.Windows;
using System.Windows.Data;
using SharpXMPP.WPF.Helpers;
using System.Collections.ObjectModel;
using SharpXMPP.WPF.Models;
using System.Windows.Documents;

namespace SharpXMPP.WPF.ViewModels
{
    public class LogCollection : ObservableCollection<Inline>
    {

    }
    public class XmlViewerViewModel : ViewModelBase
    {
        public XmlViewerViewModel()
        {
            App.DB.Log.Load();
            _log = new CollectionViewSource { Source = App.DB.Log.Local };
            Log = _log.View;
            ClearLogCommand = new DelegateCommand<RoutedEventArgs>((e) =>
                                                                       {
                                                                           App.DB.Log.RemoveRange(App.DB.Log.Local);
                                                                           App.DB.SaveChanges();
                                                                       }, () => true);
        }
        private readonly CollectionViewSource _log;
        public ICollectionView Log { get; set; }
        public DelegateCommand<RoutedEventArgs> ClearLogCommand { get; set; }
    }
}
