using CommunityToolkit.Maui.Views;
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
        double bgmVol = 0;

        public double BgmVolume => BgmVol;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SfxVolume))]
        double sfxVol = 0;

        public double SfxVolume => SfxVol;

        [RelayCommand]
        public async Task ShowIt()
        {        
            await Shell.Current.Navigation.PushModalAsync(new SettingsPage(this));
        }
    }
}
