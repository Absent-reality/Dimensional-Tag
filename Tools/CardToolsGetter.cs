#if ANDROID
using System.Threading.Tasks;
using static DimensionalTag.CardTools;
using static System.Net.Mime.MediaTypeNames;
#endif

namespace DimensionalTag.Tools
{
    public partial class CardToolsGetter
    {
#if ANDROID
        private static CardTools GetCardTools()
        {
            var acti = (MainActivity?)Platform.CurrentActivity;
               
               return acti.cardTools;                  
        }
#endif
        public static void SetOnCardReceive(NfcDataReceive cb)
        {
#if ANDROID
            var tools = GetCardTools();
            tools.OnNfcDataRecieve = cb;
#endif
        }

        public static Task WriteCard(string text, ushort Id)
        {
            var task = new TaskCompletionSource<string>();
#if ANDROID
            var cdtools = GetCardTools();
            cdtools.OnAfterNfcWrite = () =>
            {
                cdtools.OnAfterNfcWrite = null;
                task.SetResult("");
                
            };            
            cdtools.WriteItemType = text;
            cdtools.WriteItemId = Id;
#endif
            return task.Task;
        }

        public static void WriteCardCancel()
        {
#if ANDROID
            var cdtools = GetCardTools();
            cdtools.OnAfterNfcWrite = null;
#endif
        }
    }
}
