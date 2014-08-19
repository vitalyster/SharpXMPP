using SharpXMPP.WPF.Models;
using System.Data.Entity;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace SharpXMPP.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(
                CultureInfo.CurrentCulture.IetfLanguageTag)));

            base.OnStartup(e);
            Database.SetInitializer(new XMPPContextInitializer());
        }

    }
}
