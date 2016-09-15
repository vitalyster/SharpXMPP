using System.Windows;
using SharpXMPP.WPF.ViewModels;

namespace SharpXMPP.WPF.Views
{
    /// <summary>
    /// Interaction logic for XmlLog.xaml
    /// </summary>
    public partial class XmlLog : Window
    {
        public XmlLog()
        {
            InitializeComponent();
            DataContext = new XmlViewerViewModel();
        }
    }
}
