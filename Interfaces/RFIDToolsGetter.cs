#if ANDROID
using System.Threading.Tasks;
using static DimensionalTag.RFIDTools;
using static System.Net.Mime.MediaTypeNames;
#endif

namespace DimensionalTag.Interfaces
{
    public partial class RFIDToolsGetter
    {
#if ANDROID
        private static RFIDTools GetRFIDTools()
        {
            var acti = (MainActivity)Platform.CurrentActivity;
            return acti.rfidTools;
        }
#endif
        public static void SetOnRfidReceive(NfcDataReceive cb)
        {
#if ANDROID
            var rfidtoosl = GetRFIDTools();
            rfidtoosl.OnNfcDataRecieve = cb;
#endif
        }

        public static Task WriteRFIDrun(string text)
        {
            var task = new TaskCompletionSource<string>();
#if ANDROID
            var rftools = GetRFIDTools();
            rftools.OnAfterNfcWrite = () =>
            {
                rftools.OnAfterNfcWrite = null;
                task.SetResult("");
            };            
            rftools.recordString = text;
#endif
            return task.Task;
        }

        public static void WriteRFIDCancel()
        {
#if ANDROID
            var rftools = GetRFIDTools();
            rftools.OnAfterNfcWrite = null;
#endif
        }
    }
}
