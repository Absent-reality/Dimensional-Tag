using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;

namespace DimensionalTag
{
    public partial class SearchViewModel : SettingsViewModel
    {

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
                await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong.", "Ok.", "", false));
                return;
            }
            var thisItem = ListItems.FirstOrDefault(x => x.ItemName == itemName);
            if (thisItem == null) { return;}

            if (thisItem.Id == null & thisItem.ItemName != null)
            {
                World? world = World.Worlds.FirstOrDefault(m => m.Name == thisItem.ItemName);
                if (world == null)
                {
                    await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong with world.", "Ok.", "", false));
                    return;
                }

                var navParam = new Dictionary<string, object> { { "WorldParam", world } };
                await Shell.Current.GoToAsync($"///WorldsPage", navParam);
            }
            else if (thisItem.Id != null)
            {
                switch (thisItem.Id.Value)
                {
                    case <= 999:

                        Character? character = Character.Characters.FirstOrDefault(m => m.Id == thisItem.Id);
                        if (character == null)
                        {
                            await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong with character.", "Ok.", "", false));
                            return;
                        }

                        var charNavParam = new Dictionary<string, object> { { "CharacterParam", character } };
                        await Shell.Current.GoToAsync($"///CharacterPage", charNavParam);

                        break;

                    case > 999:
                        Vehicle? vehicle = Vehicle.Vehicles.FirstOrDefault(m => m.Id == thisItem.Id);
                        if (vehicle == null)
                        {
                            await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong with vehicle.", "Ok.", "", false));
                            return;
                        }

                        if (vehicle.Form == 1)
                        {
                            var vehiNavParam = new Dictionary<string, object> { { "VehicleParam", vehicle } };
                            await Shell.Current.GoToAsync($"///VehiclesPage", vehiNavParam);
                        }
                        else if (vehicle.Form == 2)
                        {
                            var vehi = Vehicle.Vehicles.FirstOrDefault(v => v.Id == vehicle.Id - 1);
                            if (vehi != null)
                            {
                                var navParam = new Dictionary<string, object> { { "VehicleParam", vehi } };
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

                        break;
                }
            }
        }
    }
}
