using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.Text;


namespace DimensionalTag;

[QueryProperty(nameof(VehicleParam), nameof(VehicleParam))]

public partial class VehiclesPage : ContentPage
{
    public Vehicle VehicleParam
    {
        set { SpinTo(value); }
    }

    public VehiclesPage(SettingsViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        sfx.BindingContext = vm;
        sfx.Source = MediaSource.FromResource("swish_rev.mp3");
        this.Loaded += Page_Loaded;
        var vehi = Vehicle.Vehicles.FindAll(x => x.Form == 1);
		vehicle_carousel.ItemsSource = vehi;   
        
	}

    void Page_Loaded(object? sender, EventArgs e)
    {

        //Only need to fire this once then we can forget it.
        this.Loaded -= Page_Loaded;

        var vm = this.BindingContext as SettingsViewModel;
        sfx.Volume = vm!.SfxVol;
        
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

    private void SpinTo(Vehicle vehicle)
    {
        var check = Vehicle.Vehicles.FirstOrDefault(x => x.Name == vehicle.Name);
        if (check != null)
        {
            int start = vehicle_carousel.Position;
            vehicle_carousel.Position = Vehicle.Vehicles.FindAll(x => x.Form == 1).IndexOf(check);
        }
    }

    private void OnArrival(object sender, NavigatedToEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = true;
        PoppingIn();
    }

    private async void OnGoodbye(object sender, NavigatedFromEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = false;

        if (sfx.CurrentState == CommunityToolkit.Maui.Core.Primitives.MediaElementState.Playing)
        {
            sfx.Stop();
        }

        await vehi_title.FadeTo(0);
        await vehicle_carousel.FadeTo(0);
    }

    private async void Vehicle_Tapped(object sender, TappedEventArgs e)
    {
        vehicle_carousel.IsEnabled = false;
#if ANDROID
        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        Vehicle? current = vehicle_carousel.CurrentItem as Vehicle;
        if (current != null)
        {

            var popup = new PopupPage(current);
            var result = await Shell.Current.ShowPopupAsync(popup);
            if (result is bool sure)
            {

                var alert = new AlertPopup(" Alert! ", " Are you sure you want to write this data? ", " Cancel?", " Write? ", true);
                var confirm = await Shell.Current.ShowPopupAsync(alert);
                if (confirm is bool tru)
                {
                    var navParam = new Dictionary<string, object> { { "WriteVehicle", current } };

                    await Shell.Current.GoToAsync($"///ScanPage", navParam);
                }
            }

        }
#endif        
        vehicle_carousel.IsEnabled = true;
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