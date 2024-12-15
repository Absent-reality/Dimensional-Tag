using CommunityToolkit.Maui.Views;
using System.Text;

namespace DimensionalTag
{

    public partial class DebugPopup : Popup
    {
      
        public DebugPopup(StringBuilder debugMessage)
        {
            DebugPopupViewModel vm = new();
            InitializeComponent();
            BindingContext = vm;
            vm.DisplayDebug(debugMessage);
            
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            Close();
        }

        private void CopyButton_Clicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        }
    }
}