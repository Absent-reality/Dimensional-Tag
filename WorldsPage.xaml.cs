using CommunityToolkit.Maui.Views;

namespace DimensionalTag;

[QueryProperty(nameof(WorldParam), nameof(WorldParam))]

public partial class WorldsPage : ContentPage
{
   
    public World WorldParam
    {
        set { Vm.SpinTo(value); }
    }

    public WorldsViewModel Vm { get; set; }
	public WorldsPage(WorldsViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        Vm = vm;
        vm.GetWorldList();

        sfx.Source = MediaSource.FromResource("swish.mp3");
      //  World_Carousel.ItemsSource = World.Worlds;
        this.Loaded += Page_Loaded;
    }

    void Page_Loaded(object? sender, EventArgs e)
    {
        this.Loaded -= Page_Loaded;
        sfx.Volume = Vm.CheckValue("Sfx", sfx.Volume);
    }

    public async void PoppingIn()
    {
        sfx.Source = MediaSource.FromResource("swish.mp3");
        //measure the display size to know how far to translate.
        var width = (DeviceDisplay.MainDisplayInfo.Width) / 2;

        await Task.Delay(400);
        await world_title.TranslateTo(-width, 0, 100);
        await world_title.FadeTo(1);
        await world_title.TranslateTo(0, 0, 250);
        sfx.Play();
        await Task.Delay(500);
        sfx.Stop();

        await Task.Delay(500);
        await World_Carousel.FadeTo(1);
        await Item_Carousel.FadeTo(1);

    }

    private void OnArrival(object sender, NavigatedToEventArgs e)
    {
        PoppingIn();      
    }

    private async void OnGoodbye(object sender, NavigatedFromEventArgs e)
    {
        if (sfx.CurrentState == CommunityToolkit.Maui.Core.Primitives.MediaElementState.Playing)
        {
            sfx.Stop();
        }

        await world_title.FadeTo(0);
        await World_Carousel.FadeTo(0);
        await Item_Carousel.FadeTo(0);
    }

    private void On_Position_Changed(object sender, PositionChangedEventArgs e)
    {
        if (sfx.CurrentState == CommunityToolkit.Maui.Core.Primitives.MediaElementState.Playing)
        {
            sfx.Stop();
        }
        sfx.Source = MediaSource.FromResource("click.mp3");
        sfx.Play();

    }
}