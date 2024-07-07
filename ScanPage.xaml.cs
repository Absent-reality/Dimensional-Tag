#if ANDROID
using Android.App;
using Android.App.AppSearch;
using Android.Nfc.Tech;
using Android.OS;
using AndroidX.ConstraintLayout.Core;
using CommunityToolkit.Maui.Views;
using DimensionalTag.Enums;
using DimensionalTag.Tools;
using Java.Lang;
using Java.Time;
using Java.Util.Concurrent;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Text;
#endif

namespace DimensionalTag
{ 
    [QueryProperty(nameof(WriteCharacter), nameof(WriteCharacter))]
    [QueryProperty(nameof(WriteVehicle), nameof(WriteVehicle))]
  
    public partial class ScanPage : ContentPage
    {

        public Character WriteCharacter
        {
            set => BeginWrite(value);
        }

        public Vehicle WriteVehicle
        {
            set => BeginWrite(value);
        }

        public bool cameToWrite = false;

        public ScanPage()
        {
            InitializeComponent();
        
#if ANDROID
            RFIDToolsGetter.SetOnRfidReceive(async (cardInfo) =>
            {
               
                if (cardInfo != null)
                {
                    LoadTo(cardInfo);
                    
                }
             
            });
            
#endif         
            
        }


        public async void LoadTo(object obj)
        {
#if ANDROID
            switch (obj)
            {
                case Character:
                    {
                        Character c = (Character)obj;
                        Character? character = Character.Characters.FirstOrDefault(m => m.Id == c.Id);

                        if (character == null) 
                        {
                            await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong with character.", "Ok.", "", false));
                            
                        }
                        else
                        {
                            var navParam = new Dictionary<string, object> { { "CharacterParam", character } };

                            await Shell.Current.GoToAsync($"///CharacterPage", navParam);
                        }
                    }
                    break;

                case Vehicle:
                    {
                        Vehicle v = (Vehicle)obj;                       
                        Vehicle? vehicle = Vehicle.Vehicles.FirstOrDefault(m => m.Id == v.Id);

                        if (vehicle == null)
                        {
                            await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong with vehicle.", "Ok.", "", false));
                        }
                        else
                        {
                            var navParam = new Dictionary<string, object> { { "VehicleParam", vehicle } };

                            await Shell.Current.GoToAsync($"///VehiclesPage", navParam);
                        }
                    }
                    break;

                case null:
                    {
                        await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong.", "Ok.", "", false));
                    }
                    break;
            }
#endif
        }

        private async void BeginWrite(object item)
        {
#if ANDROID
            await Task.Delay(500);
            switch (item)
            {
                case Character:
                    {
                        cameToWrite = true;
                        Character c = (Character)item;

                       
                        var h =  RFIDToolsGetter.WriteCard("Character", c.Id);
                        h.Wait(1000);
                        if (h.IsCompleted)
                        {
                            await Shell.Current.ShowPopupAsync(new AlertPopup("Yay!", "Write Complete.", "Ok.", "", false));
                        }
                    }
                    break;

                case Vehicle:
                    {
                        cameToWrite = true;
                        Vehicle v = (Vehicle)item;

                    }
                    break;

                case null:
                    {
                        await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong.", "Ok.", "", false));
                    }
                    break;
            }
#endif
        }

        private async void On_Arrived(object sender, NavigatedToEventArgs e)
        {
            DeviceDisplay.KeepScreenOn = true;
            await Task.Delay(600);
            if (!cameToWrite)
            {
                Lbl_scan.Text = "Place phone on tag to scan.";
                await Lbl_scan.FadeTo(1, 1000);

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
            else if (cameToWrite)
            {

                Lbl_scan.Text = "Hold phone on empty tag to write.";
                await Lbl_scan.FadeTo(1, 1000);
                img_write.IsVisible = true;
                await Task.Delay(200);
                await img_scan.TranslateTo(0, -50, 500);
                await Task.Delay(600);
                await img_scan.TranslateTo(0, 0, 500);
                await Task.Delay(500);
                await img_scan.TranslateTo(0, -50, 500);
                await Task.Delay(600);
                await img_scan.TranslateTo(0, 0, 500);
            }

        }

        private async void On_Goodbye(object sender, NavigatedFromEventArgs e)
        {
            await Lbl_scan.FadeTo(0,250);
            DeviceDisplay.KeepScreenOn = false;
            img_write.IsVisible = false;
            cameToWrite = false;
        }

    }

}

