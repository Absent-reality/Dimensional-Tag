using CommunityToolkit.Maui.Views;
using System.Text;

namespace DimensionalTag
{

    public partial class DebugPopup : Popup
    {
        private string _debugMessage = "";
        public string DebugMessage
        {
            get { return _debugMessage; }
            set
            {
                if (_debugMessage == value)
                    return;
                _debugMessage = value;
                OnPropertyChanged(nameof(DebugMessage));
            }
        }

        public DebugPopup(StringBuilder debugMessage)
        {
            InitializeComponent();
            DebugMessage = debugMessage.ToString();
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