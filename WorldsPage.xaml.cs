using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;

namespace DimensionalTag;

[QueryProperty(nameof(WorldParam), nameof(WorldParam))]

public partial class WorldsPage : ContentPage
{
   
    public World WorldParam
    {
        set { SpinTo(value); }
    }

    
	public WorldsPage(SettingsViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
        sfx.BindingContext = vm;
        sfx.Source = MediaSource.FromResource("swish.mp3");
        World_Carousel.ItemsSource = World.Worlds;
        this.Loaded += Page_Loaded;
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

    private void SpinTo(World world)
    {
        var check = World.Worlds.FirstOrDefault(x => x.Name == world.Name);
        if (check != null) 
        {
            int start = World_Carousel.Position;
            World_Carousel.Position = World.Worlds.IndexOf(check);                     
        }
    }

    private ObservableCollection<LegoTag.SearchItems> ListItems = [];
    private void world_carousel_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        //Begin the digging for matches. 
        ListItems.Clear();

       if ( e.CurrentItem != null )
        {
            World? world = e.CurrentItem as World;

            if ( world != null )
            { 
                var C = Character.Characters.FindAll(x => x.World == world.Name);
                var v = Vehicle.Vehicles.FindAll(x => x.World == world.Name);
                var V = v.FindAll(x => x.Form == 1);

                foreach (var w in C)
                {
                    ListItems.Add(new LegoTag.SearchItems() { ItemName = w.Name, Id = w.Id, Images = w.Images});
                }
                foreach (var w in V)
                {
                    ListItems.Add(new LegoTag.SearchItems() { ItemName = w.Name, Id = w.Id, Images = w.Images});
                }
             
            }
        }
      
       Item_Carousel.ItemsSource = ListItems;     

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

        await world_title.FadeTo(0);
        await World_Carousel.FadeTo(0);
        await Item_Carousel.FadeTo(0);
    }

    private async void Item_Tapped(object sender, TappedEventArgs e)
    {
        Item_Carousel.IsEnabled = false;
        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);

        if ( Item_Carousel.CurrentItem != null)
        {
            var check = Item_Carousel.CurrentItem as LegoTag.SearchItems;

            if (check == null)
            {
                await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong.", "Ok.", "", false));
            }
            else
            {
                if (check.Id == null & check.ItemName != null)
                {
                    await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong..", "Ok.", "", false));
                }
                else if (check.Id != null)
                {
                    if (check.Id.Value <= 800)
                    {
                        Character? character = Character.Characters.FirstOrDefault(m => m.Id == check.Id);
                        if (character != null)
                        {
                            var popup = new PopupPage(character);
                            var result = await Shell.Current.ShowPopupAsync(popup);
                            if (result is bool sure)
                            {
                                var alert = new AlertPopup(" Alert! ", " Are you sure you want to write this data? ", " Cancel?", " Write? ", true);
                                var confirm = await Shell.Current.ShowPopupAsync(alert);
                                if (confirm is bool tru)
                                {
                                    var navParam = new Dictionary<string, object> { { "WriteCharacter", character } };

                                    await Shell.Current.GoToAsync($"///ScanPage", navParam);
                                }
                            }
                        }
                    }
                    else if (check.Id.Value > 800)
                    {
                        Vehicle? vehicle = Vehicle.Vehicles.FirstOrDefault(m => m.Id == check.Id);
                        if (vehicle != null)
                        {
                            var popup = new PopupPage(vehicle);
                            var result = await Shell.Current.ShowPopupAsync(popup);
                            if (result is bool meh)
                            {
                                var alert = new AlertPopup(" Alert! ", " Are you sure you want to write this data? ", " Cancel?", " Write? ", true);
                                var confirm = await Shell.Current.ShowPopupAsync(alert);
                                if (confirm is bool tru)
                                {
                                    var navParam = new Dictionary<string, object> { { "WriteVehicle", vehicle } };

                                    await Shell.Current.GoToAsync($"///ScanPage", navParam);
                                }
                            }
                        }
                    }
                    else
                    {
                        await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong with Id.", "Ok.", "", false));
                    }
                }
            }
        }

        Item_Carousel.IsEnabled = true;
    }

    private void OnWorld_Position_Changed(object sender, PositionChangedEventArgs e)
    {
        if (sfx.CurrentState == CommunityToolkit.Maui.Core.Primitives.MediaElementState.Playing)
        {
            sfx.Stop();
        }
        sfx.Source = MediaSource.FromResource("click.mp3");
        sfx.Play();

    }

    private void OnItem_Position_Changed(object sender, PositionChangedEventArgs e)
    {
        if (sfx.CurrentState == CommunityToolkit.Maui.Core.Primitives.MediaElementState.Playing)
        {
            sfx.Stop();
        }
        sfx.Source = MediaSource.FromResource("click.mp3");
        sfx.Play();
    }
}