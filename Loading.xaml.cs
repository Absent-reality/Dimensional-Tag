using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Views;

namespace DimensionalTag;

public partial class Loading : ContentPage
{
	public Loading()
	{
		InitializeComponent();

        var window = App.Window;

        window.Deactivated += (s, e) =>
        {
            bgm.ShouldMute = true;
        };

        window.Activated += (s, e) =>
        {
            bgm.ShouldMute = false;
        };

        this.Loaded += Page_Loaded;
    }

    private async void Page_Loaded(object? sender, EventArgs e)
    {
        //Only need to fire this once then we can forget it.
        this.Loaded -= Page_Loaded;

        if (!Preferences.Default.ContainsKey("save"))
        {
          PoppingIn();
        }
        else
        {
            bool isSaved = Preferences.Default.Get("save", false);
            if (!isSaved) 
            { 
                PoppingIn(); 
            }            
            else
            {
                await Shell.Current.GoToAsync($"///CharacterPage");
            }
        }  
    }

    private async void PoppingIn()
    {

        if (Preferences.Default.ContainsKey("Bgm"))
        {
            double bgmVol = Preferences.Default.Get<double>("Bgm", 0);
            bgm.Volume = bgmVol;
        }

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
        await Task.Delay(3000);
        await Shell.Current.GoToAsync($"///CharacterPage");
    }

    private void OnArrival(object sender, NavigatedToEventArgs e)
	{
        DeviceDisplay.KeepScreenOn = true;
    }

    private void OnGoodbye(object sender, NavigatedFromEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = false;
        bgm.Handler?.DisconnectHandler();
    }

}