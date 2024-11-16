using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;

namespace DimensionalTag
{
    public partial class WorldsViewModel : SettingsViewModel
    {

        [ObservableProperty]
        int worldLastIndex;

        [ObservableProperty]
        int worldCenterIndex;

        [ObservableProperty]
        int itemLastIndex;

        [ObservableProperty]
        int itemFirstIndex;

        private int _itemsCollectionEnd;
        public int ItemsCollectionEnd
        {
            get { return SortedItems.Count -1; }
            set
            {
                if (_itemsCollectionEnd == value)
                    return;
                _itemsCollectionEnd = value;
                OnPropertyChanged(nameof(ItemsCollectionEnd));
            }
        }

        [ObservableProperty]
        int itemCenterIndex;

        [ObservableProperty]
        bool isEnabled = true;

        [ObservableProperty]
        World? currentWorld;

        [ObservableProperty]
        SearchItems? currentItem;      

        [ObservableProperty]
        ObservableCollection<World> _allWorlds = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ItemsCollectionEnd))]
        ObservableCollection<SearchItems> _sortedItems = new();

        private ObservableCollection<SearchItems> ListItems = [];

        public CollectionView? WorldCollection {  get; set; }

        public CollectionView? ItemCollection { get; set; }

        public void GetWorldList()
        {
            AllWorlds.Clear();
            AllWorlds.Add( new World( "", "left_placeholder.png" ));
            foreach (var world in World.Worlds)
            {
                AllWorlds.Add(world);
            }
            AllWorlds.Add(new World("", "right_placeholder.png"));
        }

        [RelayCommand]
        void WorldChanged(object world)
        {         
            SetItemsList((World)world);        
        }

        [RelayCommand]
        async Task Item_Tapped(string name)
        {         
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);

            if (name == "") { return; }
            IsEnabled = false;
            var thisItem = SortedItems.FirstOrDefault(x => x.ItemName == name);
            if (thisItem?.Id == null)
            {
                await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong..", "Ok.", "", false));
                IsEnabled = true;
                return;
            }
            switch (thisItem.Id.Value)
            {
                case <= 999:
                    {
                        Character? character = Character.Characters.FirstOrDefault(m => m.Id == thisItem.Id);
                        if (character == null) { return; }

                        var popup = new PopupPage(true, character);
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
                    break;

                case > 999:
                    {
                        Vehicle? vehicle = Vehicle.Vehicles.FirstOrDefault(m => m.Id == thisItem.Id);
                        if (vehicle == null) { return; }

                        var popup = new PopupPage(true, vehicle);
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
                    break;
            }
            ItemCollection?.ScrollTo(thisItem, position: ScrollToPosition.Center);
            IsEnabled = true;
        }

        public int GetWorldPosition(World world)
        {
            int index = AllWorlds.IndexOf(world);
            return index;
        }

        public void SetItemsList(World? world)
        {
            if (world?.Name == "") { return; }
            var thisWorld = world;
            //Begin the digging for matches. 
            ListItems.Clear();

            if (thisWorld == null) { return; }
            ListItems.Add( new SearchItems() { ItemName = "", Id = 0, Images = "left_placeholder.png" });

            var characters = Character.Characters.FindAll(x => x.World == thisWorld.Name);
            var vehi = Vehicle.Vehicles.FindAll(x => x.World == thisWorld.Name);
            var firstForm = vehi.FindAll(x => x.Form == 1);

            foreach (var w in characters)
            {
                ListItems.Add(new SearchItems() { ItemName = w.Name, Id = w.Id, Images = w.Images });
            }
            foreach (var w in firstForm)
            {
                ListItems.Add(new SearchItems() { ItemName = w.Name, Id = w.Id, Images = w.Images });
            }

            ListItems.Add(new SearchItems() { ItemName = "", Id = 0, Images = "right_placeholder.png" });

            SortedItems = ListItems;
        }
    }
}
