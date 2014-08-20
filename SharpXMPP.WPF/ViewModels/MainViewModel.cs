using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SharpXMPP.WPF.Helpers;
using SharpXMPP.WPF.Views;

namespace SharpXMPP.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            ShowLogCommand = new DelegateCommand<RoutedEventArgs>((e) =>
                                                                      {
                                                                          var l = new XmlLog();
                                                                          l.ShowDialog();
                                                                      }, () => true);
        }

        public DelegateCommand<RoutedEventArgs> ShowLogCommand { get; set; }
    }
}
