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
        [NotifyPropertyChangedFor(nameof(LastCharacter))]
        int lastIndex;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CenterCharacter))]
        int centerIndex;

        private Character? _lastCharacter;
        public Character? LastCharacter
        {
            get { _lastCharacter = Character.Characters.ElementAt(LastIndex); return _lastCharacter; }
            set {  if (_lastCharacter == value)
                    return;
                   _lastCharacter = value;
                OnPropertyChanged();
            }                
        }

        private Character? _centerCharacter;
        public Character? CenterCharacter
        {
            get { _centerCharacter = Character.Characters.ElementAt(CenterIndex); return _centerCharacter; }
            set
            {
                if (_centerCharacter == value)
                    return;
                _centerCharacter = value;
                OnPropertyChanged();
            }
        }

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

        [RelayCommand]
        async void Character_Tapped()
        {
            IsEnabled = false;
#if ANDROID || WINDOWS
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
                        LetsWriteIt("WriteCharacter", CurrentItem);
                    }
                }

            }
#endif
            IsEnabled = true;
            CurrentItem = null;
        }

        public int GetCharacterPosition(Character character)
        {
            int index;
                var check = Character.Characters.FirstOrDefault(x => x.Name == character.Name);
            if (check != null)
            {
                index = Character.Characters.IndexOf(check);
            }
            else index = -1;
            return index;   
        }
    }
}
