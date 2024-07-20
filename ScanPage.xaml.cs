
using CommunityToolkit.Maui.Views;
using DimensionalTag.Tools;


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
            mediaElement.Source = MediaSource.FromResource("lego_pieces.mp3");

#if ANDROID
            CardToolsGetter.SetOnCardReceive(async (cardInfo) =>
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
                            if (vehicle.Form == 1)
                            {
                                var navParam = new Dictionary<string, object> { { "VehicleParam", vehicle } };

                                await Shell.Current.GoToAsync($"///VehiclesPage", navParam);
                            }
                            else if (vehicle.Form == 2)
                            {
                                var veh = Vehicle.Vehicles.FirstOrDefault(v => v.Id == vehicle.Id - 1);
                                if (veh != null)
                                {
                                    var navParam = new Dictionary<string, object> { { "VehicleParam", veh } };
                                    await Shell.Current.GoToAsync($"///VehiclesPage", navParam);
                                }


                            }
                            else if (vehicle.Form == 3)
                            {
                                var V = Vehicle.Vehicles.FirstOrDefault(x => x.Id == vehicle.Id - 2);
                                if (V != null)
                                {
                                    var navParam = new Dictionary<string, object> { { "VehicleParam", V } };
                                    await Shell.Current.GoToAsync($"///VehiclesPage", navParam);
                                }

                            }                         
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

                        await Task.Run(async () =>
                        {
                            
                          await CardToolsGetter.WriteCard("Character", c.Id);
                            

                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                mediaElement.Play();
                                await Task.Delay(100);
                                mediaElement.Stop();
                                cameToWrite = false;
                                SwapBg(cameToWrite);
                              
                            });

                        });
                    }
                    break;

                case Vehicle:
                    {
                        cameToWrite = true;
                        Vehicle v = (Vehicle)item;

                        await Task.Run(async () =>
                        {
                            await CardToolsGetter.WriteCard("Vehicle", v.Id);

                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                mediaElement.Play();
                                await Task.Delay(100);
                                mediaElement.Stop();
                                cameToWrite = false;
                                SwapBg(cameToWrite);
                            });
                        });
                    }
                    break;

                case null:
                    {
                        //If navigating here from write, then to the opposite type (ie character to vehicle) 
                        // It throws null since the other info clears.
                    }
                    break;
            }
#endif
        }

        private async void On_Arrived(object sender, NavigatedToEventArgs e)
        {
            DeviceDisplay.KeepScreenOn = true;
            await Task.Delay(600);
            SwapBg(cameToWrite);

        }

        private async void On_Goodbye(object sender, NavigatedFromEventArgs e)
        {
            await Lbl_scan.FadeTo(0,250);
            DeviceDisplay.KeepScreenOn = false;
            img_write.IsVisible = false;
            cameToWrite = false;
#if ANDROID
            CardToolsGetter.WriteCardCancel();
#endif
        }

        private async void SwapBg(bool switchItUp)
        {
            switch (switchItUp)
            {
                case true:
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
                    break;

                case false:
                    {
                        img_write.IsVisible = false;
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
                    break;
            }

        }

    }

}

