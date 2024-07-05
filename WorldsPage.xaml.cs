
namespace DimensionalTag;

[QueryProperty(nameof(WorldParam), nameof(WorldParam))]

public partial class WorldsPage : ContentPage
{
   
    public World WorldParam
    {
        set { SpinTo(value); }
    }

    
	public WorldsPage()
	{
		InitializeComponent();
        World_Carousel.ItemsSource = World.Worlds;
        this.Loaded += Page_Loaded;
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
        //measure the display size to know how far to translate.
        var width = (DeviceDisplay.MainDisplayInfo.Width) / 2;

        await Task.Delay(400);
        await world_title.TranslateTo(-width, 0, 100);
        await world_title.FadeTo(1);
        await world_title.TranslateTo(0, 0, 250);

        await Task.Delay(500);
        await World_Carousel.FadeTo(1);
        await Item_Carousel.FadeTo(1);

    }

    private async void SpinTo(World world)
    {
        await Task.Delay(600);
        var check = World.Worlds.FirstOrDefault(x => x.Name == world.Name);
        if (check != null) 
        {
            int start = World_Carousel.Position;
            var number = World.Worlds.IndexOf(check);

            if (World_Carousel.Position == number)
            {
                World_Carousel.Position = number;
            }
            else if (start < number)
            {
                for (int idc = 0; idc < number - start; idc++)
                {
                    await Task.Delay(200);
                    World_Carousel.Position ++;
                }
            }
            else if (start > number)
            {
                for (int idc = start; idc > number; idc-- ) 
                { 
                    await Task.Delay(200);
                    World_Carousel.Position --; 
                }
            }                           
        }
    }
    private void world_carousel_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {

        List<string> chari = [];
        List<string> vehi = [];
       
       if ( e.CurrentItem != null )
        {
            World? world = e.CurrentItem as World;

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
        PoppingIn();
        
    }

    private void OnGoodbye(object sender, NavigatedFromEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = false;
        world_title.FadeTo(0);
        World_Carousel.FadeTo(0);
        Item_Carousel.FadeTo(0);
    }
}