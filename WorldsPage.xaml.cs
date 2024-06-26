
namespace DimensionalTag;

public partial class WorldsPage : ContentPage
{
	public WorldsPage()
	{
		InitializeComponent();
		var character = Character.Characters;
		var vehicle = Vehicle.Vehicles;
		List<string> charWorlds = [];
		List<string> vehiWorlds = [];

		foreach (var w in character)
		{
			charWorlds.Add(w.World);
		}

		foreach (var w in vehicle)
		{
			vehiWorlds.Add(w.World);
		}

		var worlds = new List<string>([.. charWorlds, .. vehiWorlds]).Distinct().ToList();
		World_Carousel.ItemsSource = worlds;
    }

    private void world_carousel_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        var World = World_Carousel.CurrentItem.ToString();
        List<string> chari = [];
        List<string> vehi = [];
        var vec = Character.Characters.FindAll(x => x.World == World);
        var test = Vehicle.Vehicles.FindAll(x => x.World == World);

        foreach (var w in vec)
        {
            chari.Add(w.Name);
        }
        foreach (var w in test)
        {
            vehi.Add(w.Name);
        }
        var items = new List<string>([.. chari, .. vehi]);
         Item_Carousel.ItemsSource = items;
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