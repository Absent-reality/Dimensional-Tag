using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DimensionalTag
{
    public partial class BaseViewModel(AppSettings settings, IAlert alert) : ObservableObject
    {

        public AppSettings Settings = settings;
        public IAlert Alert = alert;

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
        /// Sends the object on nav to page based on chosen writing device.
        /// </summary>
        /// <param name="item">Object to pass on nav</param>
        public async void LetsWriteIt(ToyTag item)
        {
            var navParam = new Dictionary<string, object> { { "WriteTag", item } };
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

        /// <summary>
        /// For passing objects on navigation.
        /// </summary>
        /// <param name="destinationPage">Name of the page to send to ie nameof(MainPage)</param>
        /// <param name="key">String key for the item sent.</param>
        /// <param name="item">Item to send.</param>
        public async void PassItOn(string destinationPage, string key, object item)
        {
            var navParam = new Dictionary<string, object> { { key, item } };              
            await Shell.Current.GoToAsync($"///{destinationPage}", navParam);     
        }
    }
}
