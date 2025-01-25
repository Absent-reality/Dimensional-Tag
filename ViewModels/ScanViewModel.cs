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

        private bool overWrite = false;
        public bool OverWrite 
        { 
            get => overWrite;
            set
            {
                if (overWrite == value) return;
                overWrite = value;
                OnPropertyChanged(nameof(OverWrite));
                nfcTools.CanOverWrite(overWrite);
            }
        }

        [ObservableProperty]
        bool canErase = false;        

        [ObservableProperty]
        string message = "";

        [ObservableProperty]
        double viewOpacity = 0;

        [ObservableProperty]
        string eraseMessage = "";

        [ObservableProperty]
        bool eraseVisible = false;

        [ObservableProperty]
        int eraseTaps = 0;

        [RelayCommand]
        void Erase_Tapped()
        {
            CanErase = false;
            switch (EraseTaps)
            {
                case 0:
                    EraseMessage = "";
                    nfcTools.WriteCardCancel();
                    break;
                case 1:
                    EraseVisible = true;
                    EraseMessage = " Warning! Tag will be Erased. \n Tap 3 more times to continue.";
                    break;
                case >1 and <4:
                    EraseMessage = $"Tapped {EraseTaps} times. {4-EraseTaps} to go.";
                    break;
                case 4:
                    EraseMessage = "Erase Enabled! Place Tag under device to erase, or tap once more to cancel.";
                    var current = EraseTag(CanErase = true);
                    EraseTaps = 0;
                    return;
            }
            EraseTaps++;
        }

        [RelayCommand]
        void CancelStuff()
        {
            OverWrite = false;
            CanErase = false;
            EraseMessage = "";
            EraseTaps = 0;
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
                            if(character.Id == 769)
                            { character = Character.Characters.FirstOrDefault(c => c.Id == 46); }
                            PassItOn(nameof(CharacterPage), "CharacterParam", character!);
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
                            Vehicle? vehicleToLoad = new(0, 0, "", "", "", []);
                            switch (vehicle.Form)
                            {

                                case 1:
                                    vehicleToLoad = vehicle;                            
                                    break;

                                case 2:
                                    vehicleToLoad = Vehicle.Vehicles.FirstOrDefault(v => v.Id == vehicle.Id - 1);
                                    break;

                                case 3:
                                    vehicleToLoad = Vehicle.Vehicles.FirstOrDefault(x => x.Id == vehicle.Id - 2);
                                    break;
                            }

                            if (vehicleToLoad != null && vehicleToLoad.Id != 0)
                            {
                                PassItOn(destination, "VehicleParam", vehicleToLoad);
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
           
            await nfcTools.SendToWrite(item);
            WriteEnabled = false;
            CancelStuff();
            return true;
        }

        async Task<bool>EraseTag(bool confirm)
        {
            var status = await nfcTools.EraseIt(confirm);
            CancelStuff();
            nfcTools.WriteCardCancel();
            return true;
        }
    }
}
