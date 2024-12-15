using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Android.Hardware.Usb;

namespace DimensionalTag
{

    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize  | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait )]
    [IntentFilter(new[] { UsbManager.ActionUsbDeviceAttached })]
    [MetaData(UsbManager.ActionUsbDeviceAttached, Resource = "@xml/device_filter")]

    public class MainActivity : MauiAppCompatActivity
    {
        public NfcTools? Tools;
        public AppSettings? Settings;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            var settings = IPlatformApplication.Current!.Services.GetService<AppSettings>();
            var iNfc  = IPlatformApplication.Current!.Services.GetService<INfcTools>();
            if (iNfc != null)
            {
                Tools = iNfc as NfcTools;
            }     
        
            base.OnCreate(savedInstanceState);      
            Tools?.OnCreate(this);
         
        }

        protected override void OnResume()
        {
            base.OnResume();
            Tools!.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();         
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            Tools!.OnNewIntent(intent!);
        }
    }
}
