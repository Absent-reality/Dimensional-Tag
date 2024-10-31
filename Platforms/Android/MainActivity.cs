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

        UsbManager? manager;
        Activity? activity;
        private const int ProductId = 0x0241;
        private const int VendorId = 0x0E6F;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            cardTools = new CardTools(this);

            manager = GetSystemService(Context.UsbService) as UsbManager;
            activity = Platform.CurrentActivity;
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

            if (intent.Action == UsbManager.ActionUsbDeviceAttached)
            {
                manager = GetSystemService(UsbService) as UsbManager;
            }

            cardTools.OnNewIntent(intent);
        }
    }
}
