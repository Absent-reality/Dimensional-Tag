#if ANDROID
using System.Threading.Tasks;
using static DimensionalTag.RFIDTools;
using static System.Net.Mime.MediaTypeNames;
#endif

namespace DimensionalTag.Tools
{
    public partial class RFIDToolsGetter
    {
#if ANDROID
        private static RFIDTools GetRFIDTools()
        {
            var acti = (MainActivity?)Platform.CurrentActivity;
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

        public static Task WriteCard(string text, ushort Id)
        {
            var task = new TaskCompletionSource<string>();
#if ANDROID
            var rftools = GetRFIDTools();
            rftools.OnAfterNfcWrite = () =>
            {
                rftools.OnAfterNfcWrite = null;
                task.SetResult("");
            };            
            rftools.WriteItemType = text;
            rftools.WriteItemId = Id;
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
