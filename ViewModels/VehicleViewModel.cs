using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;

namespace DimensionalTag
{
    public partial class VehicleViewModel : SettingsViewModel
    {
        [ObservableProperty]
        int position;

        [ObservableProperty]
        bool isEnabled = true;

        [ObservableProperty]
        Vehicle? currentItem;

        [ObservableProperty]
        ObservableCollection<Vehicle> _allVehicles = new();

        public void GetList()
        {
            AllVehicles.Clear();
            var vehicleList = Vehicle.Vehicles.FindAll(x => x.Form == 1);
            foreach (var vehicle in vehicleList)
            {
                AllVehicles.Add(vehicle);
            }
        }

        public void SpinTo(Vehicle vehicle)
        {
            var check = Vehicle.Vehicles.FirstOrDefault(x => x.Name == vehicle.Name);
            if (check != null)
            {
                Position = Vehicle.Vehicles.FindAll(x => x.Form == 1).IndexOf(check);
            }
        }

        [RelayCommand]
        async void Vehicle_Tapped()
        {
            IsEnabled = false;
#if ANDROID
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);

            if (CurrentItem != null)
            {
                var popup = new PopupPage(true, CurrentItem);
                var result = await Shell.Current.ShowPopupAsync(popup);

                if (result is bool sure)
                {
                    var alert = new AlertPopup(" Alert! ", " Are you sure you want to write this data? ", " Cancel?", " Write? ", true);
                    var confirm = await Shell.Current.ShowPopupAsync(alert);
                    if (confirm is bool tru)
                    {
                        LetsWriteIt("WriteVehicle", CurrentItem);
                    }
                }

            }
#endif
            IsEnabled = true;
        }
    }
}
