namespace DimensionalTag;

public partial class VehiclesPage : ContentPage
{
	public VehiclesPage()
	{
		InitializeComponent();
		var vehicle = Vehicle.Vehicles;
		vehicle_carousel.ItemsSource = vehicle;
	}

    private void OnArrival(object sender, NavigatedToEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = true;

    }

    private void OnGoodbye(object sender, NavigatedFromEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = false;
    }
}