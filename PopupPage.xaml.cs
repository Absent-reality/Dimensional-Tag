using CommunityToolkit.Maui.Views;

namespace DimensionalTag;

public partial class PopupPage : Popup
{ 

    public PopupPage(object sender)
	{
		InitializeComponent();
		LoadTo(sender);
	}

	public void LoadTo(object obj)
	{
		switch (obj)
		{
			case Character:
				{
					Character c = (Character)obj;
					if (c == null) {}
					else
					{
						lbl_Name.Text = c.Name;
						Img_Character.Source = c.Images;
                        lbl_World.Text = $"\n World:\n {c.World}";
						lbl_Abilities.Text = "Abilities";
						foreach (string a in c.Abilities)
						{
							edt_Abilities.Text += $"{a} \n";
						}												
					}					
				}
				break;

            case Vehicle:
                {
                    Vehicle v = (Vehicle)obj;
                    if (v == null) { }
                    else
                    {
						var form2 = Vehicle.Vehicles.FirstOrDefault(x => x.Id == v.Id + 1);
						var form3 = Vehicle.Vehicles.FirstOrDefault(x => x.Id == v.Id + 2);
						if (form2 != null) { lbl_Form2.Text = form2.Name; }
						if (form3 != null) { lbl_Form3.Text = form3.Name; }
		
						stack1.IsVisible = true;
						stack2.IsVisible = true;
                        lbl_Name.Text = v.Name;
                        Img_Character.Source = v.Images;
                        lbl_World.Text = $"\n World:\n {v.World}";
                        lbl_Abilities.Text = "Abilities";

                        foreach (string a in v.Abilities)
                        {
                            edt_Abilities.Text += $"{a} \n";
                        }
                    }
                }
                break;
				
			case null:
				break;
		}
	}

	private async void Form2_Tapped(object sender, TappedEventArgs e)
	{
        var alert = new AlertPopup(" Alert! ", " Are you sure you want to write this data? ", " Cancel?", " Write? ", true);
        var confirm = await Shell.Current.ShowPopupAsync(alert);
		var current = Vehicle.Vehicles.FirstOrDefault(x => x.Name == lbl_Form2.Text);
        if (confirm is bool tru)
        {
			if (current != null)
			{
				var navParam = new Dictionary<string, object> { { "WriteVehicle", current } };
				await Shell.Current.GoToAsync($"///ScanPage", navParam);
				Close();
			}

        }
    }

	private async void Form3_Tapped(object sender, TappedEventArgs e)
	{
        var alert = new AlertPopup(" Alert! ", " Are you sure you want to write this data? ", " Cancel?", " Write? ", true);
        var confirm = await Shell.Current.ShowPopupAsync(alert);
        var current = Vehicle.Vehicles.FirstOrDefault(x => x.Name == lbl_Form3.Text);
        if (confirm is bool tru)
        {
            if (current != null)
            {
                var navParam = new Dictionary<string, object> { { "WriteVehicle", current } };
                await Shell.Current.GoToAsync($"///ScanPage", navParam);
				Close();
            }

        }
    }

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
		Close();
    }

    private void WriteButton_Clicked(object sender, EventArgs e)
    {
		CloseAsync(true);
    }

}