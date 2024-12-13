using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DimensionalTag
{
    public partial class SettingsViewModel(AppSettings settings, IAlert alert) : BaseViewModel(settings, alert)
    {
        public IAlert Alerts { get; set; } = alert;
        public AppSettings AppSettings { get; set; } = settings;

        [RelayCommand]
        void MuteBgm()
        {
            Settings.Bgm_isMute = !Settings.Bgm_isMute;
        }
        [RelayCommand]
        void MuteSfx()
        {
            Settings.Sfx_isMute = !Settings.Sfx_isMute;
        }

    }
}
