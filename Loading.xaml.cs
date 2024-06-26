namespace DimensionalTag;

public partial class Loading : ContentPage
{
	public Loading()
	{
		InitializeComponent();
	}

	private async void OnArrival(object sender, NavigatedToEventArgs e)
	{
        await Task.Delay(1000);
        await Img_grd.FadeTo(1, 1000); // Fade out
        lbl_txt.IsVisible = false;
        await Stuff.FadeTo(1, 1000);
        for (int idc = 1; idc < 24; idc++)
        {

            Img_grd.Source = $"logo{idc}.png";
            await Task.Delay(100);

        }

        for (int idc = 1; idc < 24; idc++)
        {

            Img_grd.Source = $"logo{idc}.png";
            await Task.Delay(100);

        }
        for (int idc = 24; idc < 32; idc++)
        {

            Img_grd.Source = $"logo{idc}.png";
            await Task.Delay(50);

        }
        for (int idc = 31; idc < 55; idc++)
        {

            Img_grd.Source = $"logo{idc}.png";
            await Task.Delay(100);

        }
        for (int idc = 55; idc < 60; idc++)
        {

            Img_grd.Source = $"logo{idc}.png";
            await Task.Delay(50);

        }
        for (int idc = 60; idc < 66; idc++)
        {

            Img_grd.Source = $"logo{idc}.png";
            await Task.Delay(100);

        }

        await Task.Delay(2000);
        await Img_grd.FadeTo(0, 1000);
        Img_grd.Source = "gear_logo.png";
        Img_grd.Scale = .50;
        await Img_grd.FadeTo(1, 1000);
        await lbl_txt.FadeTo(1, 1000);
        lbl_txt.IsVisible = true;
        await Stuff.FadeTo(1, 1000);
        await Task.Delay(1000);
        await Stuff.FadeTo(0, 1000);
        await lbl_txt.FadeTo(0, 250);
        await Img_grd.FadeTo(0, 250);
        Img_grd.Source = "logo1.png";
        Img_grd.Scale = 1;
        await Shell.Current.GoToAsync("///CharacterPage");
    }
}