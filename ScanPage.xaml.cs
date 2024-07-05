#if ANDROID
using Android.App;
using Android.Nfc.Tech;
using Android.OS;
using AndroidX.ConstraintLayout.Core;
using CommunityToolkit.Maui.Views;
using DimensionalTag.Enums;
using DimensionalTag.Interfaces;
using Java.Util.Concurrent;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Text;
#endif
namespace DimensionalTag
{
    public partial class ScanPage : ContentPage
    {

        public ScanPage()
        {
            InitializeComponent();
#if ANDROID
            RFIDToolsGetter.SetOnRfidReceive(async (cardInfo) =>
            {
                message = cardInfo;
                if (cardInfo != "")
                {
                   await ShowIt(cardInfo);
                }
             
            });
#endif         
            
        }
        public string message = "";

        private async Task ShowIt(string message)
        {
#if ANDROID
            await Shell.Current.ShowPopupAsync(new AlertPopup("Surprise!", message, "Ok", "", false));
#endif
        }

        private async void On_Arrived(object sender, NavigatedToEventArgs e)
        {
            DeviceDisplay.KeepScreenOn = true;

            Lbl_scan.Text = "Place phone on tag to scan."; 
            await Lbl_scan.FadeTo(1,1000); 
            
            for (int idc = 1; idc < 10; idc++)
            {
                img_scan.Source = "scan_one.png";
                await Task.Delay(200);
                img_scan.Source = "scan_two.png";
                await Task.Delay(200);
                img_scan.Source = "scan_three.png";
                await Task.Delay(200);
                img_scan.Source = "scan_four.png";
                await Task.Delay(200);
            }
        }

        private async void On_Goodbye(object sender, NavigatedFromEventArgs e)
        {
            await Lbl_scan.FadeTo(0,250);
            DeviceDisplay.KeepScreenOn = false;
        }

    }

}

