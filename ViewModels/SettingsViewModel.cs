﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DimensionalTag.Enums;
using DimensionalTag.Tools;

namespace DimensionalTag
{
    public partial class SettingsViewModel : ObservableObject
    {
        public Settings Settings { get; private set; }

        public SettingsViewModel()
        {
            Settings = Settings.GetInstance();
            RestoreSettings();
        }

        [ObservableProperty]
        ImageSource bgmImg = "volume_icon.png";

        [ObservableProperty]
        ImageSource sfxImg = "volume_icon.png";

        [RelayCommand]
        public async Task ShowIt()
        {
            await Shell.Current.Navigation.PushModalAsync(new SettingsPage(this));
        }

        [RelayCommand]
        void MuteBgm()
        {
            if (!Settings.Bgm_isMute)
            {
                BgmImg = "mute_icon.png";
                Settings.Bgm_isMute = true;
            }
            else
            {
                BgmImg = "volume_icon.png";
                Settings.Bgm_isMute = false;
            }
        }
        [RelayCommand]
        void MuteSfx()
        {
            if (!Settings.Sfx_isMute)
            {
                SfxImg = "mute_icon.png";
                Settings.Sfx_isMute = true;
            }
            else
            {
                SfxImg = "volume_icon.png";
                Settings.Sfx_isMute = false;
            }
        }

        void RestoreSettings()
        {
            if (Preferences.ContainsKey("Bgm"))
            {
                Settings.Bgm_Volume = Preferences.Default.Get<double>("Bgm", 0);
            }

            if (Preferences.ContainsKey("Sfx"))
            {
                Settings.Sfx_Volume = Preferences.Default.Get<double>("Sfx", 0);
            }

            if (Preferences.ContainsKey("save"))
            {
                Settings.Save = Preferences.Default.Get<bool>("save", false);
            }

            if (Preferences.ContainsKey("WritingType"))
            {
                var result = Preferences.Default.Get<bool>("WritingType", false);               
                switch (result)
                {
                    case (true): Settings.WritingType = true; Settings.SetWritingDevice = WritingDevice.Portal; break;
                        case (false): Settings.WritingType = false; Settings.SetWritingDevice = WritingDevice.Nfc; break;
                }           
            }
               
        }

        public void OnActivated()
        {
            DeviceDisplay.KeepScreenOn = true;
            Settings.Bgm_isMute = false;
            Settings.Sfx_isMute = false;
        }

        public void OnDeactivated()
        {
            DeviceDisplay.KeepScreenOn = false;
            Settings.Bgm_isMute = true;
            Settings.Sfx_isMute = true;
        }

        public double CheckValue(string name, double value)
        {
            switch (name)
            {
                case "Bgm":
                    if (value != Settings.Bgm_Volume)
                    { return Settings.Bgm_Volume; }
                    break;

                case "Sfx":
                    if (value != Settings.Sfx_Volume)
                    { return Settings.Sfx_Volume; }
                    break;

            }
            return value;
        }

        /// <summary>
        /// Sends the object on nav to page specified
        /// </summary>
        /// <param name="key">string for ref passed on nav</param>
        /// <param name="item">object to pass on nav</param>
        public async void LetsWriteIt( string key, object item )
        {        
            var navParam = new Dictionary<string, object> { { key, item } };            
            switch (Settings.SetWritingDevice)
            {
                case WritingDevice.Nfc:
                    await Shell.Current.GoToAsync($"///ScanPage", navParam);
                    break;

                case WritingDevice.Portal:
                    await Shell.Current.GoToAsync($"///PortalPage", navParam);
                    break;
            }
        }
    }
}
