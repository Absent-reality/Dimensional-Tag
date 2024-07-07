using Android.App;
using Android.Content;
using Android.Nfc;
using CommunityToolkit.Maui.Views;
using DimensionalTag.Tools;

namespace DimensionalTag
{
    public class RFIDTools
    {
        private NfcAdapter? _nfcAdaper;
        private Activity act;

        public delegate void AfterNFCwrite();
        public AfterNFCwrite OnAfterNfcWrite;

        public ushort WriteItemId;
        public string WriteItemType;
        public RFIDTools(Activity actIn)
        {
            act = actIn;
            _nfcAdaper = NfcAdapter.GetDefaultAdapter(act);
        }

        public NfcDataReceive OnNfcDataRecieve;

        public void OnResume()
        {
            if (_nfcAdaper == null)
            {
                ErrorReport("Nfc Unavailable", "Nfc is not supported on this device.");
            }
            else
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
            }
        }

        public void OnNewIntent(Intent intent)
        {
            if (intent.Action != NfcAdapter.ActionTagDiscovered) return;

            if (OnAfterNfcWrite != null)
            {

              //  RFID_Simple.PrepareToWrite(intent, WriteItemId, WriteItemType);
                OnAfterNfcWrite();
                return;
            }

            var cardInfo = RFID_Simple.TryRead(intent);
                       
            if (OnNfcDataRecieve != null)
            {
                OnNfcDataRecieve(cardInfo: cardInfo);
                
            }
        }

        private static Task ErrorReport(string title, string message) => Shell.Current.ShowPopupAsync(new AlertPopup( title, message, "Ok", "", false));

    }
}
