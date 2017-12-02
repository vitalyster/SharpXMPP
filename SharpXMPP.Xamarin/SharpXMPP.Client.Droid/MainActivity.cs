using Android.App;
using Android.Widget;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;

namespace SharpXMPP.Client.Droid
{
    [Activity(
        Label = "SharpXMPP.Client.Droid",
        MainLauncher = true,
        Theme = "@style/MyTheme"
        )]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            FormsAppCompatActivity.ToolbarResource = Resource.Layout.Toolbar;
            FormsAppCompatActivity.TabLayoutResource = Resource.Layout.Tabbar;

            base.OnCreate(savedInstanceState);

            Forms.SetFlags("FastRenderers_Experimental");
            Forms.Init(this, savedInstanceState);

            LoadApplication(new App());
        }
    }
}

