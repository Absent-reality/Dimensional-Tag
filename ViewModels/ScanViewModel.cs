using CommunityToolkit.Maui.Animations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DimensionalTag
{
    public partial class ScanViewModel(AppSettings settings, IAlert alert, INfcTools nfcTools) : BaseViewModel(settings, alert)
    {
        public IAlert Alerts { get; set; } = alert;
        public AppSettings AppSettings { get; set; } = settings;

        [ObservableProperty]
        bool writeEnabled = false;

        [ObservableProperty]
        bool overWrite = false;

        [ObservableProperty]
        string message = "";

        [ObservableProperty]
        double viewOpacity = 0;

        [RelayCommand]
        void OverWriteTag()
        {
            OverWrite = false;
        }

        public async void LoadTo(ToyTag toy)
        {
            switch (toy.ToyTagType)
            {
                case ToyTagType.Character:
                    {
                        Character? character = Character.Characters.FirstOrDefault(m => m.Id == toy.Id);

                        if (character == null)
                        {
                            await Alert.SendAlert("Oops...", "Something went wrong with character.", "Ok.", "", false);
                        }
                        else
                        {
                            PassItOn(nameof(CharacterPage), "CharacterParam", character);
                        }
                    }
                    break;

                case ToyTagType.Vehicle:
                    {
                        Vehicle? vehicle = Vehicle.Vehicles.FirstOrDefault(m => m.Id == toy.Id);

                        if (vehicle == null)
                        {
                            await Alert.SendAlert("Oops...", "Something went wrong with vehicle.", "Ok.", "", false);
                        }
                        else
                        {
                            string destination = nameof(VehiclesPage);
                            Vehicle? vehicleToLoad;
                            switch (vehicle.Form)
                            {

                                case 1:
                                  
                                    PassItOn(destination, "VehicleParam", vehicle);
                                    break;

                                case 2:
                                   
                                    vehicleToLoad = Vehicle.Vehicles.FirstOrDefault(v => v.Id == vehicle.Id - 1);
                                    if (vehicleToLoad != null)
                                    {
                                        PassItOn(destination, "VehicleParam", vehicleToLoad);
                                    }
                                    break;

                                case 3:
                                    vehicleToLoad = Vehicle.Vehicles.FirstOrDefault(x => x.Id == vehicle.Id - 2);
                                    if (vehicleToLoad != null)
                                    {
                                        PassItOn(destination, "VehicleParam", vehicleToLoad);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        public async Task<bool>BeginWrite(ToyTag item)
        {
            if (item.Name == "") { return false; }
            if(item.ToyTagType == ToyTagType.Vehicle)
            WriteEnabled = true; 
           
            await nfcTools.SendToWrite(item, OverWrite);
            WriteEnabled = false;
            OverWrite = false;
            return true;
        }

        public void FadeIt()
        {
           
        }
    }
}
