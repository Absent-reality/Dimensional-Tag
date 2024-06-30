
namespace DimensionalTag;

public partial class WorldsPage : ContentPage
{
	public WorldsPage()
	{
		InitializeComponent();

        var worlds = World.Worlds;
		World_Carousel.ItemsSource = worlds;
    }

    private void world_carousel_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {

        List<string> chari = [];
        List<string> vehi = [];
       
       if ( e.CurrentItem != null )
        {
            var world = e.CurrentItem as World;

            if ( world != null )
            { 
                var C = Character.Characters.FindAll(x => x.World == world.Name);
                var V = Vehicle.Vehicles.FindAll(x => x.World == world.Name);

                foreach (var w in C)
                {
                   chari.Add(w.Name);
                }
                foreach (var w in V)
                {
                   vehi.Add(w.Name);
                }
             
            }
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