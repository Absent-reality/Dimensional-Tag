using CommunityToolkit.Maui.Views;

namespace DimensionalTag
{

    public partial class AlertPopup : Popup
    {
        /// <summary>
        /// Display an alert message.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="cancelButton">Button text for cancel. I.e."ok"</param>
        /// <param name="confirmButton">Button text for confirm. I.e."yes"</param>
        /// <param name="confirmVisible">For confirm button to be visible.</param>
        public AlertPopup(string title, string text, string cancelButton, string confirmButton, bool confirmVisible)
        {
            AlertPopupViewModel vm = new();            
            InitializeComponent();
            BindingContext = vm;
            Vm = vm;
            vm.Title = title;
            vm.Text = text;
            vm.CancelButton = cancelButton;
            vm.ConfirmButton = confirmButton;
            vm.IsVisible = confirmVisible;
            vm.Popup = this;
        }

        public AlertPopupViewModel Vm { get; set; }

        private async void BtnWrite_Clicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            await CloseAsync(true);
        }
        private void BtnCancel_Clicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            Close();
        }
    }
}