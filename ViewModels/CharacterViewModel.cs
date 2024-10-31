using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;

namespace DimensionalTag
{

    public partial class CharacterViewModel : SettingsViewModel
    {

        [ObservableProperty]
        int position;

        [ObservableProperty]
        bool isEnabled = true;

        [ObservableProperty]
        Character? currentItem;

        [ObservableProperty]
        ObservableCollection<Character> _allCharacters = new();

        public void GetList()
        {
            AllCharacters.Clear();
            foreach (var character in Character.Characters)
            {
                AllCharacters.Add(character);
            }
        }

        public void SpinTo(Character character)
        {
            var check = Character.Characters.FirstOrDefault(x => x.Name == character.Name);
            if (check != null)
            {
                Position = Character.Characters.IndexOf(check);
            }
        }

        [RelayCommand]
        async void Character_Tapped()
        {
            IsEnabled = false;
#if ANDROID
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);

            if (CurrentItem != null)
            {
                var popup = new PopupPage(CurrentItem);
                var result = await Shell.Current.ShowPopupAsync(popup);

                if (result is bool sure)
                {
                    var alert = new AlertPopup(" Alert! ", " Are you sure you want to write this data? ", " Cancel?", " Write? ", true);
                    var confirm = await Shell.Current.ShowPopupAsync(alert);
                    if (confirm is bool tru)
                    {
                        LetsWriteIt("WriteCharacter", CurrentItem);
                    }
                }

            }
#endif
            IsEnabled = true;
        }
    }
}
