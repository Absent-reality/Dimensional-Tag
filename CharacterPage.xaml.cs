using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;

namespace DimensionalTag;

[QueryProperty(nameof(CharacterParam), nameof(CharacterParam))]

public partial class CharacterPage : ContentPage
{
    public Character CharacterParam
    {
        set { Vm.SpinTo(value); }
    }

    public CharacterViewModel Vm {  get; set; }

    public CharacterPage(CharacterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        Vm = vm;
        vm.GetList();

        sfx.Source = MediaSource.FromResource("swish.mp3");
        bgm.Source = MediaSource.FromResource("Defender_Main.mp3");

        var window = App.Window;
        window.Destroying += (s, e) =>
        {
            bgm.Handler?.DisconnectHandler();
            sfx.Handler?.DisconnectHandler();
        };

        this.Loaded += Page_Loaded;
    }
      
    void Page_Loaded(object? sender, EventArgs e)
    {
        this.Loaded -= Page_Loaded;      
        bgm.Volume = Vm.CheckValue("Bgm", bgm.Volume);
        sfx.Volume = Vm.CheckValue("Sfx", sfx.Volume);
    }

    public async void PoppingIn()
    {
        sfx.Source =  MediaSource.FromResource("swish.mp3");
         //measure the display size to know how far to translate.
        var width = (DeviceDisplay.MainDisplayInfo.Width)/2;

        await Task.Delay(500);
        await char_title.TranslateTo(-width, 0, 100);
        await char_title.FadeTo(1);
        char_title.IsVisible = true;
        await char_title.TranslateTo(0, 0, 250);
        sfx.Play();
        await Task.Delay(500);
        sfx.Stop();

        await Task.Delay(500);
        await carousel.FadeTo(1);       
    }

    private void OnArrival(object sender, NavigatedToEventArgs e)
    {
        PoppingIn();        
    }

    private async void OnGoodbye(object sender, NavigatedFromEventArgs e)
    {
        if (sfx.CurrentState == MediaElementState.Playing)
        {
            sfx.Stop();
        }

        await char_title.FadeTo(0);
        await carousel.FadeTo(0);
    }

    private void OnPosition_Changed(object sender, PositionChangedEventArgs e) 
    {
        if (sfx.CurrentState == MediaElementState.Playing)
        {
            sfx.Stop();
        }
        sfx.Source = MediaSource.FromResource("click.mp3");
        sfx.Play();             
    }
}