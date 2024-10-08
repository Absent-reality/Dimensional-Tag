using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;


namespace DimensionalTag;

public partial class SearchPage : ContentPage
{
	public SearchPage()
	{
		InitializeComponent();

        this.Loaded += Page_Loaded;

	}

    void Page_Loaded(object? sender, EventArgs e)
    {
        //Only need to fire this once then we can forget it.
        this.Loaded -= Page_Loaded;

        //Call our animation.
        PoppingIn();
    }

    public void PoppingIn()
    {
        //measure the content size
        var contentSize = this.Content.Measure(Window.Width, Window.Height, MeasureFlags.IncludeMargins);
        var contentHeight = contentSize.Request.Height;

        //Start by translating the content below /off screen
        this.Content.TranslationY = contentHeight;

        //Animate the translucent background and fade.
        this.Animate("Background",
            callback: v => this.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v)),
            start: 0d,
            end: 0.7d,
            rate: 32,
            length: 350,
            easing: Easing.CubicOut,
            finished: (v, k) =>
                   this.Background = new SolidColorBrush(Colors.Black.WithAlpha(0.7f)));

        //Also animate the content sliding up from below the screen
        this.Animate("Content",
            callback: v => this.Content.TranslationY = (int)(contentHeight - v),
            start: 0,
            end: contentHeight,
            length: 500,
            easing: Easing.CubicInOut,
            finished: (v, k) => this.Content.TranslationY = 0);
    }

    public Task PoppingOut()
    {
        var done = new TaskCompletionSource();

        //Measure our content size so we know how much to translate
        var contentSize = this.Content.Measure(Window.Width, Window.Height, MeasureFlags.IncludeMargins);
        var windowHeight = contentSize.Request.Height;

        //Start fading out the background
        this.Animate("Background",
            callback: v => this.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v)),
            start: 0.7d,
            end: 0d,
            rate: 32,
            length: 350,
            easing: Easing.CubicIn,
            finished: (v, k) =>
                   this.Background = new SolidColorBrush(Colors.Black.WithAlpha(0.0f)));

        //Start Sliding the content back down below the bottom of the screen
        this.Animate("Content",
           callback: v => this.Content.TranslationY = (int)(windowHeight - v),
           start: windowHeight,
           end: 0,
           length: 500,
           easing: Easing.CubicInOut,
           finished: (v, k) =>
           {
               this.Content.TranslationY = windowHeight;
               // important: Set out completion source to done!
               done.TrySetResult();
           });
    
        return done.Task;
    }

    async Task Close()
    {
        //Wait for animation to finish
        await PoppingOut();
        //Nav away without the default animation
        await Navigation.PopModalAsync(animated: false);
    }

    private ObservableCollection<LegoTag.SearchItems> ListItems = [];
   
    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
       //Begin the digging for matches. 
        ListItems.Clear();

        bool notFound = false;
		string query = e.NewTextValue;

        if (query != "")
        {
            var items = LegoTag.SearchTags(query);

            if (items == null || items.Count == 0)
            {
                notFound = true;
                ListItems.Add(new LegoTag.SearchItems() { ItemName = "Name not found." });
            }
            else
            {
                notFound = false;
                foreach(var item in items)
                ListItems.Add(new LegoTag.SearchItems() { ItemName = item.ItemName, Id = item.Id });
            }
 
            if (notFound) { lbl_results.Text = $" Results: {0} "; }
            else
            {    
               var count = ListItems.Where(x => !string.IsNullOrWhiteSpace(x.ItemName)).Count();
               lbl_results.Text = $" Results: {count} ";
            }           
                 searchResults.ItemsSource = ListItems;
            
        }
        else 
        {
            lbl_results.Text = "";
            searchResults.ItemsSource = null;  
        }
            
    }

    private async void BtnCancel_Clicked(object sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        await Close();
    }

    private void On_Arrived(object sender, NavigatedToEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = true;
        searchBar.Text = string.Empty;
    }

    private async void Tapped_Outside(object sender, TappedEventArgs e)
    {
        await Close();
    }

    private void On_Goodbye(object sender, NavigatedFromEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = false;
    }

    private async void SearchResults_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        if (e.SelectedItem != null)
        {          
            var check = e.SelectedItem as LegoTag.SearchItems;
            if ( check == null )
            {
                await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong.", "Ok.", "", false));
            }
            else
            {                
                if ( check.Id == null & check.ItemName != null )
                {
                    World? world = World.Worlds.FirstOrDefault( m => m.Name == check.ItemName );
                    if ( world == null )
                    {                     
                        await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong with world.", "Ok.", "", false));
                    }
                    else
                    {
                        var navParam = new Dictionary<string, object> { { "WorldParam", world } };

                        await Shell.Current.GoToAsync($"///WorldsPage", navParam);
                    }
                    
                }
                else if ( check.Id != null )
                {
                    if ( check.Id.Value <= 800 )
                    {
                       Character? character = Character.Characters.FirstOrDefault( m => m.Id == check.Id );
                       if ( character == null ) 
                        {
                            await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong with character.", "Ok.", "", false));
                        }
                        else
                        {
                            var navParam = new Dictionary<string, object> { { "CharacterParam", character } };

                            await Shell.Current.GoToAsync( $"///CharacterPage", navParam );                           
                        }
                    }
                    else if ( check.Id.Value > 800 )
                    {                  
                       Vehicle? vehicle = Vehicle.Vehicles.FirstOrDefault( m => m.Id == check.Id );
                        if ( vehicle == null ) 
                        {
                            await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong with vehicle.", "Ok.", "", false));
                        }
                        else
                        {
                            if ( vehicle.Form == 1 )
                            {
                               var navParam = new Dictionary<string, object> { { "VehicleParam", vehicle } };

                               await Shell.Current.GoToAsync( $"///VehiclesPage", navParam );
                            }
                            else if ( vehicle.Form == 2 )
                            {
                                var veh = Vehicle.Vehicles.FirstOrDefault(v => v.Id == vehicle.Id - 1);
                                if ( veh != null)
                                { 
                                var navParam = new Dictionary<string, object> { { "VehicleParam", veh } };
                                await Shell.Current.GoToAsync($"///VehiclesPage", navParam);
                                }
                            }
                            else if ( vehicle.Form == 3 )
                            {
                                var V = Vehicle.Vehicles.FirstOrDefault(x => x.Id == vehicle.Id - 2);
                                if (V != null)
                                {
                                    var navParam = new Dictionary<string, object> { { "VehicleParam", V } };
                                    await Shell.Current.GoToAsync($"///VehiclesPage", navParam);
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
    }
}