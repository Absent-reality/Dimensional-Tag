using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DimensionalTag
{
     public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Saved))]
        bool save;       

        public bool Saved => Save;

        [ObservableProperty]
        double bgmVol = 0;

        [ObservableProperty]
        double sfxVol = 0;

        [ObservableProperty]
        ImageSource bgmImg = "volume_icon.png";

        [ObservableProperty]
        ImageSource sfxImg = "volume_icon.png";

        [ObservableProperty]
        bool bgmIsMuted = false;

        [ObservableProperty]
        bool sfxIsMuted = false;

        [RelayCommand]
        public async Task ShowIt()
        {        
            await Shell.Current.Navigation.PushModalAsync(new SettingsPage(this));
        }

        [RelayCommand]
        void MuteBgm()
        {
            if (!BgmIsMuted)
            {
                BgmImg = "mute_icon.png";
                BgmIsMuted = true;
            }
            else
            {
                BgmImg = "volume_icon.png";
                BgmIsMuted = false;
            }
        }
        [RelayCommand]
        void MuteSfx()
        {
            if (!SfxIsMuted)
            {
                SfxImg = "mute_icon.png";
                SfxIsMuted = true;
            }
            else
            {
                SfxImg = "volume_icon.png";
                SfxIsMuted = false;
            }
        }

    }
}
