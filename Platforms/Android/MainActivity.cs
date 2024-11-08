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
        public CardTools cardTools;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            cardTools = new CardTools(this);

            cardTools.OnCreate();
        }

        protected override void OnResume()
        {
            base.OnResume();
            cardTools.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();         
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            cardTools.OnNewIntent(intent);
        }
    }
}
