using CommunityToolkit.Maui.Views;

namespace DimensionalTag;

public partial class AlertPopup : Popup
{ 
    /// <summary>
    /// Display an alert message.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="text"></param>
    /// <param name="cancelButton">Button text for cancel. I.e."ok"</param>
    /// <param name="confirmButton">Button text for confirm. I.e."yes"</param>
    /// <param name="confirm">For confirm button to be visible.</param>
	public AlertPopup(string title, string text, string cancelButton, string confirmButton, bool confirm)
	{
		InitializeComponent();
        lbl_Title.Text = title;
        edt_Alert.Text = text;
        btn_Cancel.Text = cancelButton;
        btn_Confirm.Text = confirmButton;
        btn_Confirm.IsVisible = confirm;
	}

    private async void BtnWrite_Clicked(object sender, EventArgs e)
    {
        await CloseAsync(true);
    }
    private void BtnCancel_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}