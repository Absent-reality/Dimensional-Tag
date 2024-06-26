using Android.App;
using Android.Content.PM;
using Android.Nfc;
using Android.OS;
using Android.Content;
using AndroidX.Core.Content;

namespace DimensionalTag
{

    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public RFIDTools rfidTools;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            rfidTools = new RFIDTools(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            rfidTools.OnResume();
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            rfidTools.OnNewIntent(intent);
        }
    }
}
