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
            bool notFound = false;

            if (Query != "")
            {

                var items = SearchItems.SearchTags(Query);

                if (items == null || items.Count == 0)
                {
                    notFound = true;
                    ListItems.Add(new SearchItems() { ItemName = "Name not found." });
                }
                else
                {
                    notFound = false;
                    foreach (var item in items)
                    ListItems.Add(new SearchItems() { ItemName = item.ItemName, Id = item.Id });
                }

                if (notFound) { Results = $" Results: {0} "; }
                else
                {
                    var count = ListItems.Where(x => !string.IsNullOrWhiteSpace(x.ItemName)).Count();
                    Results = $" Results: {count} ";
                }
                
            }
            else
            {
                Results = "";
                ListItems = [];
            }
        }

        [RelayCommand]
        async void ItemSelected()
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);

            if (SelectedItem == null)
            {
                await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong.", "Ok.", "", false));
                return;
            }

            if (SelectedItem.Id == null & SelectedItem.ItemName != null)
            {
                World? world = World.Worlds.FirstOrDefault(m => m.Name == SelectedItem.ItemName);
                if (world == null)
                {
                    await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong with world.", "Ok.", "", false));
                    return;
                }

                var navParam = new Dictionary<string, object> { { "WorldParam", world } };
                await Shell.Current.GoToAsync($"///WorldsPage", navParam);                
            }

            else if (SelectedItem.Id != null)
            {
                switch (SelectedItem.Id.Value)
                {
                    case <= 800:

                        Character? character = Character.Characters.FirstOrDefault(m => m.Id == SelectedItem.Id);
                        if (character == null)
                        {
                            await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong with character.", "Ok.", "", false));
                            return;
                        }

                        var charNavParam = new Dictionary<string, object> { { "CharacterParam", character } };
                        await Shell.Current.GoToAsync($"///CharacterPage", charNavParam);

                        break;

                    case > 800:
                        Vehicle? vehicle = Vehicle.Vehicles.FirstOrDefault(m => m.Id == SelectedItem.Id);
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

                        break;
                }
            }
        }
    }
}
