using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DimensionalTag
{
    public partial class AlertPopupViewModel : ObservableObject
    {
        [ObservableProperty]
         string title = "";

        [ObservableProperty]
         string _text = "";

        [ObservableProperty]
         string _cancelButton = "Ok";

        [ObservableProperty]
         string _confirmButton = "";

        [ObservableProperty]
         bool _isVisible = false;

        public AlertPopup? Popup { get; set; }
    }
}
