using System.ComponentModel;
using System.Data.Entity;
using System.Windows;
using System.Windows.Data;
using SharpXMPP.WPF.Helpers;

namespace SharpXMPP.WPF.ViewModels
{
    public class XmlViewerViewModel : ViewModelBase
    {
        public XmlViewerViewModel()
        {
            Db.Log.Load();
            _log = new CollectionViewSource {Source = Db.Log.Local};
            Log = CollectionViewSource.GetDefaultView(Db.Log.Local);
            ClearLogCommand = new DelegateCommand<RoutedEventArgs>((e) =>
                                                                       {
                                                                           Db.Log.RemoveRange(Db.Log.Local);
                                                                           Db.SaveChanges();
                                                                           Db.Log.Load();
                                                                           Log.Refresh();
                                                                       }, () => true);
        }
        private readonly CollectionViewSource _log;
        public ICollectionView Log { get; set; }
        public DelegateCommand<RoutedEventArgs> ClearLogCommand { get; set; }
    }
}
