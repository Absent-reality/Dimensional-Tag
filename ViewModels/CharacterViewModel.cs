using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;

namespace DimensionalTag
{

    public partial class CharacterViewModel(AppSettings settings, IAlert alert) : BaseViewModel(settings, alert)
    {
        public IAlert Alerts { get; set; } = alert;
        public AppSettings AppSettings { get; set; } = settings;

        [ObservableProperty]    
        int lastIndex;

        [ObservableProperty]
        int centerIndex;

        [ObservableProperty]
        bool isEnabled = true;

        [ObservableProperty]
        Character? currentItem;

        [ObservableProperty]
        ObservableCollection<Character> _allCharacters = new();

        public CollectionView? cv {  get; set; }

        public void GetList()
        {
            AllCharacters.Clear();
            AllCharacters.Add(new Character(0, "", "", "left_placeholder.png", []));
            foreach (var character in Character.Characters)
            {
                AllCharacters.Add(character);
            }
            AllCharacters.Add(new Character(0, "", "", "right_placeholder.png", []));
        }

        [RelayCommand]
        async Task Character_Tapped(string name)
        {

            if (name == "") { return; }
            IsEnabled = false;

            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);

            var thisItem = Character.Characters.FirstOrDefault(c => c.Name == name);
            if (thisItem == null) { return; }

            var popup = new PopupPage(true, thisItem);
            var result = await Shell.Current.ShowPopupAsync(popup);

            if (result is bool sure)
            {
                var confirm = await Alert.SendAlert(" Alert! ", " Are you sure you want to write this data? ", " Cancel?", " Write? ", true);
                if (confirm)
                {
                    ToyTag item = ToyTag.ConvertTo(thisItem);
                    LetsWriteIt(item);
                }
            }
            cv?.ScrollTo(GetCharacterPosition(thisItem), position: ScrollToPosition.Center);
            IsEnabled = true;
        }

        public int GetCharacterPosition(Character character)
        {
            int index = AllCharacters.IndexOf(character);
            return index;   
        }
    }
}
