using CommunityToolkit.Maui.Views;

namespace DimensionalTag;

public partial class PopupPage : Popup
{
    public bool HereToWrite
    {
        get { return Vm.HereToWrite; }
        set { if (Vm.HereToWrite == value)
                return;
            Vm.HereToWrite = value;
            OnPropertyChanged(nameof(HereToWrite));
        }
    }

    public PopupViewModel Vm {  get; set; }

    public PopupPage(bool write, object sender)
	{
        PopupViewModel vm = new();
		InitializeComponent();
        BindingContext = vm;
        Vm = vm;

		vm.LoadTo(sender);
        HereToWrite = write;
        Vm.Popup = this;
	}

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        Close();
    }

    private void WriteButton_Clicked(object sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        CloseAsync(true);
    }

}