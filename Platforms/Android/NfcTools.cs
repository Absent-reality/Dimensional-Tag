using Android.App;
using Android.Content;
using Android.Nfc;
using CommunityToolkit.Maui.Views;

namespace DimensionalTag
{
    public class NfcTools(IAlert alert, AppSettings settings) : INfcTools
    {
        private NfcAdapter? _nfcAdaper;
        public Activity? act;
        public event EventHandler<NfcTagEventArgs>? NfcTagEvent;
        public ToyTag? ToyTagToWrite = null;
        private IAlert Alert = alert;
        private AppSettings Settings = settings;
        private bool OverWrite = false;
        private bool WriteComplete = true;
        private ProgressStatus WriteStatus;

        public void OnCreate(Activity actIn)
        {     
            act = actIn;
            _nfcAdaper = NfcAdapter.GetDefaultAdapter(act);
            if (_nfcAdaper == null)
            {
                Settings.NfcEnabled = false;
                Alert.SendAlert("Nfc Unavailable", "Nfc is not supported on this device. Please connect portal to usb port.", "Ok", "", false);
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
                if (!_nfcAdaper.IsEnabled) 
                   { Alert.SendAlert("Oops.", "Nfc is not enabled. Please enable from your device settings.", "Ok", "", false); }
            }
        }

        public async void OnNewIntent(Intent intent)
        {
            if (intent.Action != NfcAdapter.ActionTagDiscovered) return;
            NfcCardUtil nfcCardUtil = new(Alert);

            if (ToyTagToWrite != null)
            {
                nfcCardUtil.OverWriteTag = OverWrite;
                WriteStatus = await nfcCardUtil.PrepareToWrite(intent, ToyTagToWrite);
                ToyTagToWrite = null;
                nfcCardUtil.OverWriteTag = false;              
                WriteComplete = true;

                var shouldDebug = await ReportIt(NfcTask.Write, WriteStatus);
                if (shouldDebug)
                {                   
                    await Shell.Current.ShowPopupAsync(new DebugPopup(nfcCardUtil.ForDebug));
                }
                return;
            }
        
            var progressStatus = await nfcCardUtil!.TryRead(intent);
            if (progressStatus == ProgressStatus.Success)
            {
                if (nfcCardUtil.ReadTag != null)
                {
                    NfcTagEvent?.Invoke(this, new NfcTagEventArgs(nfcCardUtil.ReadTag));
                }
            }
            else 
            {
                var shouldDebug = await ReportIt(NfcTask.Read, progressStatus);
                if (shouldDebug)
                {
                    await Shell.Current.ShowPopupAsync(new DebugPopup(nfcCardUtil.ForDebug));
                }
            }
        }

        public async Task<ProgressStatus>SendToWrite(ToyTag toyTag)
        {
            ToyTagToWrite = toyTag;
            WriteComplete = false;

            while (!WriteComplete)
            {
               await Task.Delay(500);
            }
            return WriteStatus;
        }

        public void WriteCardCancel()
        {
            ToyTagToWrite = null;
            OverWrite = false;
        }

        public void CanOverWrite(bool confirm)
        {
            OverWrite = confirm;
        }

        private async Task<bool> ReportIt(NfcTask readOrWrite, ProgressStatus currentStatus)
        {
                    
            var (title, message, cancelText, confirmText, debug) = ("", "", "Ok.", "", false);                    
            
            switch (readOrWrite)
            {
                case NfcTask.Read:
                    switch (currentStatus)
                    {
                        case ProgressStatus.Failed:
                            title = "Oops..";
                            message = "Failed to read tag. \n Make sure tag is valid.";
                            confirmText = "View Log?";
                            debug = true;
                            break;

                        case ProgressStatus.NoConnection:
                            title = "Oops..";
                            message = "Failed to connect to tag. \n Please try again.";
                            confirmText = "View Log?";
                            debug = true;
                            break;

                        case ProgressStatus.EmptyData:
                            title = "Oops..";
                            message = "Tag doesnt contain lego data or may be empty.";
                            confirmText = "View Log?";
                            debug = true;
                            break;
                    }
                    break;

                case NfcTask.Write:
                    switch (currentStatus)
                    {
                        case ProgressStatus.Success:
                            title = "Hurray!";
                            message = "Write complete.";
                            break;

                        case ProgressStatus.Failed:
                            title = "Oops..";
                            message = "Failed to write to tag.";
                            confirmText = "View Log?";
                            debug = true;
                            break;

                        case ProgressStatus.NoConnection:
                            title = "Oops..";
                            message = "Failed to connect to tag. \n Please try again.";
                            confirmText = "View Log?";
                            debug = true;
                            break;

                        case ProgressStatus.DataExists:
                            title = "Oops..";
                            message = "Tag is not empty. \n Please use a blank tag.";
                            confirmText = "View Log?";
                            debug = true;
                            break;

                        case ProgressStatus.IncorrectType:
                            title = "Oops..";
                            message = "Tag type is not compatible. \n Please use a N213 tag.";
                            confirmText = "View Log?";
                            debug = true;
                            break;
                    }
                    break;
            }

            bool result = await Alert.SendAlert(title, message, cancelText, confirmText, debug);
            return result;
        }

        private enum NfcTask
        {
            Read,
            Write
        }
    }
}
