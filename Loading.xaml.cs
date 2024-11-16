
namespace DimensionalTag;

public partial class Loading : ContentPage
{

    public LoadingViewModel Vm;
	public Loading(LoadingViewModel vm)
	{
		InitializeComponent();
        Vm = vm;
        var window = App.Window;
        window.Deactivated += (s, e) => vm.OnDeactivated();
        window.Activated += (s, e) => vm.OnActivated();

        this.Loaded += Page_Loaded;
    }

    private async void Page_Loaded(object? sender, EventArgs e)
    {
        //Only need to fire this once then we can forget it.
        this.Loaded -= Page_Loaded;
        bgm.Volume = Vm.CheckValue("Bgm", bgm.Volume);
        sfx.Volume = Vm.CheckValue("Sfx", sfx.Volume);

        if (!Preferences.Default.ContainsKey("save"))
        {
          PoppingIn();
        }
        else
        {
            if (!Vm.Settings.Save) 
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

        await Task.Delay(1000);
        await Img_grd.FadeTo(1, 1000); // Fade out
        lbl_txt.IsVisible = false;

#if ANDROID
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
#endif
#if WINDOWS
        Img_grd.Source = $"logoani.gif";
        await Stuff.FadeTo(1, 1000);
        Img_grd.IsAnimationPlaying = true;      
        await Task.Delay(9600);
        await Img_grd.FadeTo(0, 1000);
#endif

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

    private void OnGoodbye(object sender, NavigatedFromEventArgs e)
    {
        bgm.Handler?.DisconnectHandler();
        sfx.Handler?.DisconnectHandler();
    }

}