using CommunityToolkit.Maui.Views;

namespace DimensionalTag;

[QueryProperty(nameof(VehicleParam), nameof(VehicleParam))]

public partial class VehiclesPage : ContentPage
{
    public Vehicle VehicleParam
    {
        set { Vm.SpinTo(value); }
    }

    public VehicleViewModel Vm { get; set; }
    public VehiclesPage(VehicleViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        Vm = vm;
        vm.GetList();

        sfx.Source = MediaSource.FromResource("swish_rev.mp3");
        this.Loaded += Page_Loaded;      
	}

    void Page_Loaded(object? sender, EventArgs e)
    {
        this.Loaded -= Page_Loaded;
        sfx.Volume = Vm.CheckValue("Sfx", sfx.Volume);        
    }

    public async void PoppingIn()
    { 
        sfx.Source = MediaSource.FromResource("swish_rev.mp3");
        var width = (DeviceDisplay.MainDisplayInfo.Width)/2;
        await Task.Delay(500);
        await vehi_title.TranslateTo(width, 0, 100);
        await vehi_title.FadeTo(1);
        await vehi_title.TranslateTo(0, 0, 250);
        sfx.Play();
        await Task.Delay(500);
        sfx.Stop();
        await Task.Delay(500);
        await vehicle_carousel.FadeTo(1);
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

        await vehi_title.FadeTo(0);
        await vehicle_carousel.FadeTo(0);
    }

    private void OnPosition_Changed(object sender, PositionChangedEventArgs e)
    {
        if (sfx.CurrentState == CommunityToolkit.Maui.Core.Primitives.MediaElementState.Playing)
        {
            sfx.Stop();
        }
        sfx.Source = MediaSource.FromResource("click.mp3");
        sfx.Play();
    }
}