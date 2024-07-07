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
                        lbl_Name.Text = v.Name;
                       // Img_Character.Source = v.Images;
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

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
		Close();
    }

    private void WriteButton_Clicked(object sender, EventArgs e)
    {
		CloseAsync(true);
    }
}