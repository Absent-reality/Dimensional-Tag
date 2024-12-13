
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace DimensionalTag
{
    public partial class AppSettings : ObservableObject, INotifyPropertyChanged
    {

        private double _bgm_Volume = 1;
        private double _sfx_Volume = 1;
        private WritingDevice _setWritingDevice;
        private bool _writingType;
        private bool _bgm_isMute = false;
        private bool _sfx_isMute = false;
        private bool _save;
        private bool _nfcEnabled = true;

        private const string _versionNumber = "Ver: 2.0.0";
        public string VersionNumber => _versionNumber;

        public double Bgm_Volume
        {
            get { return _bgm_Volume; }
            set
            {
                if (_bgm_Volume == value)
                    return;
                _bgm_Volume = value;
                OnPropertyChanged(nameof(Bgm_Volume));
                OnPropertyChanged(nameof(Get_BgmVolume));
            }
        }

        public double Get_BgmVolume { get { return _bgm_Volume; } }
        public double Get_SfxVolume { get { return _sfx_Volume; } }

        public double Sfx_Volume
        {
            get { return _sfx_Volume; }
            set
            {
                if (_sfx_Volume == value)
                    return;
                _sfx_Volume = value;
                OnPropertyChanged(nameof(Sfx_Volume));
                OnPropertyChanged(nameof(Get_SfxVolume));
            }
        }

        public bool Bgm_isMute
        {
            get { return _bgm_isMute; }
            set
            {
                if (_bgm_isMute == value)
                    return;
                _bgm_isMute = value;
                OnPropertyChanged(nameof(Bgm_isMute));
            }
        }

        public bool Sfx_isMute
        {
            get { return _sfx_isMute; }
            set
            {
                if (_sfx_isMute == value)
                    return;
                _sfx_isMute = value;
                OnPropertyChanged(nameof(Sfx_isMute));
            }
        }

        public bool Save
        {
            get { return _save; }
            set
            {
                if (_save == value)
                    return;
                _save = value;
                OnPropertyChanged(nameof(Save));
            }
        }

        public bool WritingType
        {
            get
            {
                if (SetWritingDevice == WritingDevice.Portal) { _writingType = true; }
                else { _writingType = false; }
                return _writingType;

            }

            set
            {
                if (value == _writingType) return;
                if (value) { SetWritingDevice = WritingDevice.Portal; _writingType = value; }
                else { SetWritingDevice = WritingDevice.Nfc; _writingType = value; }
            }
        }

        public WritingDevice SetWritingDevice
        {
            get
            { return _setWritingDevice; }
            set
            {
                if (_setWritingDevice == value)
                    return;
                _setWritingDevice = value;
                OnPropertyChanged(nameof(SetWritingDevice));
                OnPropertyChanged(nameof(WritingType));
            }
        }

        public bool NfcEnabled
        {
            get { return _nfcEnabled; }
            set
            {
                if (_nfcEnabled == value) return;
                _nfcEnabled = value;
                OnPropertyChanged(nameof(NfcEnabled));
            }
        }

        public void RestoreSettings()
        {
            if (Preferences.ContainsKey("Bgm"))
            {
                Bgm_Volume = Preferences.Default.Get<double>("Bgm", 1);
            }

            if (Preferences.ContainsKey("Sfx"))
            {
                Sfx_Volume = Preferences.Default.Get<double>("Sfx", 1);
            }

            if (Preferences.ContainsKey("save"))
            {
                Save = Preferences.Default.Get<bool>("save", false);
            }

            if (Preferences.ContainsKey("WritingType"))
            {
                var result = Preferences.Default.Get<bool>("WritingType", false);
                switch (result)
                {
                    case (true): WritingType = true; SetWritingDevice = WritingDevice.Portal; break;
                    case (false): WritingType = false; SetWritingDevice = WritingDevice.Nfc; break;
                }
            }

        }

    }
}
