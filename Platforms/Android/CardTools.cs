using Android.App;
using Android.Content;
using Android.Nfc;
using CommunityToolkit.Maui.Views;
using DimensionalTag.Tools;
using DimensionalTag.Enums;

namespace DimensionalTag
{
    public class CardTools
    {
        private NfcAdapter? _nfcAdaper;
        private Activity act;

        public delegate void AfterNFCwrite();
        public AfterNFCwrite OnAfterNfcWrite;
        public Settings? Settings;
        public ushort WriteItemId;
        public string WriteItemType;
        public CardTools(Activity actIn)
        {
            act = actIn;
            _nfcAdaper = NfcAdapter.GetDefaultAdapter(act);
        }

        public NfcDataReceive OnNfcDataRecieve;

        public void OnCreate()
        {
            if (_nfcAdaper == null)
            {
                Settings = Settings.GetInstance();
                Settings.NfcEnabled = false;
                ErrorReport("Nfc Unavailable", "Nfc is not supported on this device. Please connect portal to usb port.");
                Settings.SetWritingDevice = WritingDevice.Portal;

            }
        }
        public void OnResume()
        {
            if (_nfcAdaper != null)
            {
                //Set events and filters
                var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
                var ndefDetected = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
                var techDetected = new IntentFilter(NfcAdapter.ActionTechDiscovered);
                var filters = new[] { ndefDetected, tagDetected, techDetected };
                var intent = new Intent(act, act.GetType()).AddFlags(ActivityFlags.SingleTop);
                var pendingIntent = PendingIntent.GetActivity(act, 0, intent, PendingIntentFlags.Mutable);

                //Gives your current foreground activity priority in receving Nfc events over all other activities.
                _nfcAdaper.EnableForegroundDispatch(act, pendingIntent, filters, null);
                if (!_nfcAdaper.IsEnabled) { ErrorReport("Oops.", "Nfc is not enabled. Please enable from your device settings."); }
            }
        }

        public void OnNewIntent(Intent intent)
        {
            if (intent.Action != NfcAdapter.ActionTagDiscovered) return;

            if (OnAfterNfcWrite != null)
            {
                Card_Play.PrepareToWrite(intent, WriteItemId, WriteItemType);
                OnAfterNfcWrite();
                return;
            }

            var cardInfo = Card_Play.TryRead(intent);

            OnNfcDataRecieve?.Invoke(cardInfo: cardInfo);
        }

        private static Task ErrorReport(string title, string message) => Shell.Current.ShowPopupAsync(new AlertPopup( title, message, "Ok", "", false));

    }
}
