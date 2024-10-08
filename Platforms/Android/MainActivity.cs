﻿using Android.App;
using Android.Content.PM;
using Android.Nfc;
using Android.OS;
using Android.Content;
using AndroidX.Core.Content;
using CommunityToolkit.Maui.Alerts;

namespace DimensionalTag
{

    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize  | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait )]
    public class MainActivity : MauiAppCompatActivity
    {
        public CardTools cardTools;       

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            cardTools = new CardTools(this);
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
