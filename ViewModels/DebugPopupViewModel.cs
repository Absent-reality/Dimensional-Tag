using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DimensionalTag.Portal;
using System.Text;


namespace DimensionalTag
{
    public partial class DebugPopupViewModel : ObservableObject
    {
        [ObservableProperty]
        string _debugMessage = "";

        [ObservableProperty]
        private StringBuilder _message = new();

        public void DisplayDebug(StringBuilder debug)
        {
            Message = debug;
            DebugMessage = Message.ToString();
        }

        [RelayCommand]
        async Task CopyDebug()
        {
            if (DebugMessage == "") { return; }
            if (Clipboard.Default.HasText)
            {
                await ClearClipboard();
            }
            await Clipboard.Default.SetTextAsync(Message.ToString());
        }

        private async Task ClearClipboard()
        {
            await Clipboard.Default.SetTextAsync(null);
        }    
           
        
       
    }
}
