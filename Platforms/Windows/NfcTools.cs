
namespace DimensionalTag
{     
    public class NfcTools : INfcTools //This class is a placeholder
    {

        public void CanOverWrite(bool confirm)
        {
            //to be added
        }

        public event EventHandler<NfcTagEventArgs>? NfcTagEvent;

        public async Task<ProgressStatus> SendToWrite(ToyTag toyTag)
        {
            //To be added
            return ProgressStatus.NoConnection;
        }
       
        public void WriteCardCancel()
        {
            //To be added
        }
    }
}
