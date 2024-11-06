using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;

namespace DimensionalTag
{
    public partial class VehicleViewModel : SettingsViewModel
    {
        [ObservableProperty]
        int lastIndex;

        [ObservableProperty]
        int centerIndex;

        [ObservableProperty]
        bool isEnabled = true;

        [ObservableProperty]
        Vehicle? currentItem;

        [ObservableProperty]
        ObservableCollection<Vehicle> _allVehicles = new();

        public CollectionView? cv { get; set; }

        public void GetList()
        {
            AllVehicles.Clear();
            var vehicleList = Vehicle.Vehicles.FindAll(x => x.Form == 1);
            foreach (var vehicle in vehicleList)
            {
                AllVehicles.Add(vehicle);
            }
        }

        [RelayCommand]
        async Task Vehicle_Tapped(string name)
        {
            IsEnabled = false;
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);

            if (name == "") { return; }

            var thisItem = Vehicle.Vehicles.FirstOrDefault(v => v.Name == name);
            if (thisItem == null) { return; }

            var popup = new PopupPage(true, thisItem);
            var result = await Shell.Current.ShowPopupAsync(popup);

            if (result is bool sure)
            {
                var alert = new AlertPopup(" Alert! ", " Are you sure you want to write this data? ", " Cancel?", " Write? ", true);
                var confirm = await Shell.Current.ShowPopupAsync(alert);
                if (confirm is bool tru)
                {
                    LetsWriteIt("WriteVehicle", thisItem);
                }
            }
            cv?.ScrollTo(GetVehiclePosition(thisItem), position: ScrollToPosition.Center);
            IsEnabled = true;
        }

        public int GetVehiclePosition(Vehicle vehicle)
        { 
            int index = AllVehicles.IndexOf(vehicle);
            return index;
        }    
    }
}
