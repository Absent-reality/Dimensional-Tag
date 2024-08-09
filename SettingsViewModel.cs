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
        [NotifyPropertyChangedFor(nameof(BgmVolume))]
        double bgmVol = 1;
        
        public double BgmVolume { get { return BgmVol; } }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SfxVolume))]
        double sfxVol = 1;

        public double SfxVolume { get { return SfxVol; } }

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

        [RelayCommand]
        public void RestoreSettings()
        {
            if (Preferences.Default.ContainsKey("save"))
            {
                bool isSaved = Preferences.Default.Get("save", false);
                if (isSaved) { Save = true; }
                else { Save = false; }
            }
            if (Preferences.Default.ContainsKey("Bgm"))
            {
                double bgmVol = Preferences.Default.Get<double>("Bgm", 0);
                BgmVol = bgmVol;
            }
            if (Preferences.Default.ContainsKey("Sfx"))
            {
                double sfxVol = Preferences.Default.Get<double>("Sfx", 0);
                SfxVol = sfxVol;
            }

        }

    }
}
