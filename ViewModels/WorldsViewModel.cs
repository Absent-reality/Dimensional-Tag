using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;

namespace DimensionalTag
{
    public partial class WorldsViewModel : SettingsViewModel
    {

        [ObservableProperty]
        int position;

        [ObservableProperty]
        bool isEnabled = true;

        [ObservableProperty]
        World? currentWorld;

        [ObservableProperty]
        SearchItems? currentItem;

        [ObservableProperty]
        ObservableCollection<World> _allWorlds = new();

        [ObservableProperty]
        ObservableCollection<SearchItems> _sortedItems = new();

        private ObservableCollection<SearchItems> ListItems = [];

        public void GetWorldList()
        {
            AllWorlds.Clear();
            foreach (var world in World.Worlds)
            {
                AllWorlds.Add(world);
            }
        }

        public void SpinTo(World world)
        {
            var check = World.Worlds.FirstOrDefault(x => x.Name == world.Name);
            if (check != null)
            {
                Position = World.Worlds.IndexOf(check);
            }
        }

        [RelayCommand]
        void WorldChanged()
        {
            //Begin the digging for matches. 
            ListItems.Clear();

            if (CurrentWorld != null)
            {

                var characters = Character.Characters.FindAll(x => x.World == CurrentWorld.Name);
                var vehi = Vehicle.Vehicles.FindAll(x => x.World == CurrentWorld.Name);
                var firstForm = vehi.FindAll(x => x.Form == 1);

                foreach (var w in characters)
                {
                    ListItems.Add(new SearchItems() { ItemName = w.Name, Id = w.Id, Images = w.Images });
                }
                foreach (var w in firstForm)
                {
                    ListItems.Add(new SearchItems() { ItemName = w.Name, Id = w.Id, Images = w.Images });
                }

            }
            SortedItems = ListItems;
        }

        [RelayCommand]
        async void Item_Tapped()
        {
            IsEnabled = false;
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);

            if (CurrentItem == null)
            {
                await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong.", "Ok.", "", false));
            }
            else
            {
                if (CurrentItem.Id == null)
                {
                    await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong..", "Ok.", "", false));
                    IsEnabled = true;
                    return;
                }
                switch (CurrentItem.Id.Value)
                {
                    case <= 800:
                        {
                            Character? character = Character.Characters.FirstOrDefault(m => m.Id == CurrentItem.Id);
                            if (character != null)
                            {
                                var popup = new PopupPage(character);
                                var result = await Shell.Current.ShowPopupAsync(popup);
                                if (result is bool sure)
                                {
                                    var alert = new AlertPopup(" Alert! ", " Are you sure you want to write this data? ", " Cancel?", " Write? ", true);
                                    var confirm = await Shell.Current.ShowPopupAsync(alert);
                                    if (confirm is bool tru)
                                    {
                                        LetsWriteIt("WriteCharacter", character);
                                    }
                                }
                            }
                        }
                        break;

                    case > 800:
                        {
                            Vehicle? vehicle = Vehicle.Vehicles.FirstOrDefault(m => m.Id == CurrentItem.Id);
                            if (vehicle != null)
                            {
                                var popup = new PopupPage(vehicle);
                                var result = await Shell.Current.ShowPopupAsync(popup);
                                if (result is bool meh)
                                {
                                    var alert = new AlertPopup(" Alert! ", " Are you sure you want to write this data? ", " Cancel?", " Write? ", true);
                                    var confirm = await Shell.Current.ShowPopupAsync(alert);
                                    if (confirm is bool tru)
                                    {
                                        LetsWriteIt("WriteVehicle", vehicle);
                                    }
                                }
                            }
                        }
                        break;
                }
                
            }
          IsEnabled = true;
        }   
        
    }
}
