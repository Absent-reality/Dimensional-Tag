using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;

namespace DimensionalTag
{
    public partial class SearchViewModel(AppSettings settings, IAlert alert) : BaseViewModel(settings, alert)
    {
        public IAlert Alerts { get; set; } = alert;
        public AppSettings AppSettings { get; set; } = settings;

        private string _query = "";
        public string Query
        {
            get { return _query; }
            set
            {
                if (_query == value) return;
                _query = value;
                OnPropertyChanged();
                QueryItems();
            }
        }

        [ObservableProperty]
        string results = "";

        [ObservableProperty]
        SearchItems? selectedItem;

        [ObservableProperty]
        ObservableCollection<SearchItems> _listItems = [];

        [RelayCommand]
        void QueryItems()
        {
            //Begin the digging for matches. 
            ListItems.Clear();

            if (Query == "")
            {
                Results = "";
                ListItems = [];
                return;
            }

            var items = SearchItems.SearchTags(Query);

            if (items == null || items.Count == 0)
            {
                ListItems.Add(new SearchItems() { ItemName = "Name not found." });
                Results = $" Results: {0} ";
            }
            else
            {              
                ListItems = items;
                Results = $" Results: {ListItems.Count} ";
            }
        }

        [RelayCommand]
        async Task ItemSelected(string itemName)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);

            if (itemName == "")
            {
                await Alert.SendAlert("Oops...", "Something went wrong.", "Ok.", "", false);
                return;
            }
            var thisItem = ListItems.FirstOrDefault(x => x.ItemName == itemName);
            if (thisItem == null) { return;}

            if (thisItem.Id == null & thisItem.ItemName != null)
            {
                World? world = World.Worlds.FirstOrDefault(m => m.Name == thisItem.ItemName);
                if (world == null)
                {
                    await Alert.SendAlert("Oops...", "Something went wrong with world.", "Ok.", "", false);
                    return;
                }

                PassItOn(nameof(WorldsPage), "WorldParam", world);
            }
            else if (thisItem.Id != null)
            {
                switch (thisItem.Id.Value)
                {
                    case <= 999:

                        Character? character = Character.Characters.FirstOrDefault(m => m.Id == thisItem.Id);
                        if (character == null)
                        {
                            await Alert.SendAlert("Oops...", "Something went wrong with character.", "Ok.", "", false);
                            return;
                        }

                        PassItOn(nameof(CharacterPage), "CharacterParam", character );
                        break;

                    case > 999:
                        Vehicle? vehicle = Vehicle.Vehicles.FirstOrDefault(m => m.Id == thisItem.Id);
                        if (vehicle == null)
                        {
                            await Alert.SendAlert("Oops...", "Something went wrong with vehicle.", "Ok.", "", false);
                            return;
                        }

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
                        break;
                }
            }
        }
    }
}
