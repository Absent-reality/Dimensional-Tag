using Android.App;
using Android.Content;
using Android.Nfc;
using DimensionalTag.Interfaces;

namespace DimensionalTag
{
    public class RFIDTools
    {
        private NfcAdapter? _nfcAdaper;
        private Activity act;

        public delegate void AfterNFCwrite();
        public AfterNFCwrite OnAfterNfcWrite;

        public string recordString;
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
                
                var alert = new AlertDialog.Builder(act).Create();
                alert.SetMessage("Nfc is not supported on this device.");
                alert.SetTitle("Nfc Unavailable");
                alert.Show();
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

               // RFID_Simple.BeginWrite(intent, 41, "Character");
                OnAfterNfcWrite();
                return;
            }

            var cardInfo = RFID_Simple.TryRead(intent);
                       
            if (OnNfcDataRecieve != null)
            {
                OnNfcDataRecieve(cardInfo: cardInfo);
            }
        }
    }
}
