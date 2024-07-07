using CommunityToolkit.Maui.Views;


namespace DimensionalTag;

[QueryProperty(nameof(VehicleParam), nameof(VehicleParam))]

public partial class VehiclesPage : ContentPage
{
    public Vehicle VehicleParam
    {
        set { SpinTo(value); }
    }

    public VehiclesPage()
	{
		InitializeComponent();
        this.Loaded += Page_Loaded;       
		vehicle_carousel.ItemsSource = Vehicle.Vehicles;
	}

    void Page_Loaded(object? sender, EventArgs e)
    {
        //Only need to fire this once then we can forget it.
        this.Loaded -= Page_Loaded;

        //Call our animation.
        PoppingIn();

    }

    public async void PoppingIn()
    {
        var width = (DeviceDisplay.MainDisplayInfo.Width)/2;
        await Task.Delay(500);
        await vehi_title.TranslateTo(width, 0, 100);
        await vehi_title.FadeTo(1);
        await vehi_title.TranslateTo(0, 0, 250);
        
        await Task.Delay(500);
        await vehicle_carousel.FadeTo(1);

    }

    private async void SpinTo(Vehicle vehicle)
    {
        await Task.Delay(800);
        var check = Vehicle.Vehicles.FirstOrDefault(x => x.Name == vehicle.Name);
        if (check != null)
        {
            int start = vehicle_carousel.Position;
            var number = Vehicle.Vehicles.IndexOf(check);

            if (vehicle_carousel.Position == number)
            {
                vehicle_carousel.Position = number;
            }
            else if (start < number)
            {
                for (int idc = 0; idc < number - start; idc++)
                {
                    await Task.Delay(250);
                    vehicle_carousel.Position++;
                }
            }
            else if (start > number)
            {
                for (int idc = start; idc > number; idc--)
                {
                    await Task.Delay(250);
                    vehicle_carousel.Position--;
                }
            }
        }
    }

    private void OnArrival(object sender, NavigatedToEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = true;
        PoppingIn();
    }

    private void OnGoodbye(object sender, NavigatedFromEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = false;
        vehi_title.FadeTo(0);
        vehicle_carousel.FadeTo(0);
    }

    private async void Vehicle_Tapped(object sender, TappedEventArgs e)
    {
#if ANDROID
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

    }
}