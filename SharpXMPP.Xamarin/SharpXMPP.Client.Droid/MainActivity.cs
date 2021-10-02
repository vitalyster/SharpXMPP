using Android.App;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Net;
using System.Linq;
using System.Net;
using System.Collections.Generic;

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
            ToolbarResource = Resource.Layout.Toolbar;
            TabLayoutResource = Resource.Layout.Tabbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            // Workaround for Android, remove when System.Dns will be fixed
            DetectNameServers();
            LoadApplication(new App());
        }

        /// <summary>
        /// Read DNS resolver address using Android API
        /// Credits to https://github.com/MiniDNS/minidns/blob/master/minidns-android21/src/main/java/org/minidns/dnsserverlookup/android21/AndroidUsingLinkProperties.java
        /// </summary>
        public void DetectNameServers()
        {
            var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                // Android 6.0+
                if (connectivityManager.ActiveNetwork != null)
                {
                    var linkProperties = connectivityManager.GetLinkProperties(connectivityManager.ActiveNetwork);
                    if (linkProperties != null)
                    {
                        XmppTcpConnection.NameServers = linkProperties.DnsServers.Select(address => IPAddress.Parse(address.HostAddress)).ToArray();
                    }
                }
            }
            else
            {
                // Fallback to Android 5.0 API (Android < 5.0 will work without workaround)
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    var linkProperties = connectivityManager.GetAllNetworks().Select(network => connectivityManager.GetLinkProperties(network));
                    var servers = new List<IPAddress>();
                    foreach (var link in linkProperties)
                    {
                        var addresses = link.DnsServers.Select(address => IPAddress.Parse(address.HostAddress));
                        if (link.Routes.Any(route => route.IsDefaultRoute))
                        {
                            servers.InsertRange(0, addresses);
                        }
                        else
                        {
                            servers.AddRange(addresses);
                        }
                    }
                    XmppTcpConnection.NameServers = servers.ToArray();
                }
            }
        }
    }
}

