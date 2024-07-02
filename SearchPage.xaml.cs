using Java.Lang;
using System.Collections.ObjectModel;
using System.Globalization;

namespace DimensionalTag;

public partial class SearchPage : ContentPage
{
	public SearchPage()
	{
		InitializeComponent();

        this.Loaded += Page_Loaded;

	}

    void Page_Loaded(object sender, EventArgs e)
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
        //we now return the task so we can wait for the animation to finish.
        return done.Task;
    }

    async Task Close()
    {
        //Wait for animation to finish
        await PoppingOut();
        //Nav away without the default animation
        await Navigation.PopModalAsync(animated: false);
    }

    public class ListItem
    {
        public string? ItemName { get; set; }
        public ushort? Id { get; set; }
       
    }

    private ObservableCollection<ListItem> ListItems = [];
    
    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
       
        ListItems.Clear();

        bool notFound = false;
		string query = e.NewTextValue;

        if (query != "")
        {
            var items = LegoTag.SearchTags(query);

            if (items == null || items.Count == 0)
            {
                notFound = true;
                ListItems.Add(new ListItem() { ItemName = "Name not found." });
            }
            else
            {
                notFound = false;
                foreach(var item in items)
                ListItems.Add(new ListItem() { ItemName = item.ItemName, Id = item.Id });
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

    private async void btnCancel_Clicked(object sender, EventArgs e)
    {
        await Close();
    }

    private void On_Arrived(object sender, NavigatedToEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = true;
        searchBar.Text = string.Empty;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Close();
    }

    private void On_Goodbye(object sender, NavigatedFromEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = false;
    }

    private async void searchResults_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
              
        if (e.SelectedItem != null)
        {          
            var check = e.SelectedItem as ListItem;
            if ( check == null )
            {
                await DisplayAlert("Oops...", "Something went wrong.", "Ok.");
            }
            else
            {                
                if (check.Id == null & check.ItemName != null)
                {
                    World? world = World.Worlds.FirstOrDefault(m => m.Name == check.ItemName);
                    if ( world == null )
                    {
                        await DisplayAlert("Oops...", "Something went wrong with world.", "Ok.");
                    }
                    else
                    {
                        var navParam = new ShellNavigationQueryParameters
                        {
                            {"World", world }
                        };
                        await Shell.Current.GoToAsync($"///WorldsPage", navParam);
                    }
                    
                }
                else if (check.Id != null)
                {
                    if(check.Id.Value <= 800)
                    {
                       Character? character = Character.Characters.FirstOrDefault(m => m.Id == check.Id);
                       if (character == null) 
                        {
                            await DisplayAlert("Oops...", "Something went wrong with character.", "Ok.");
                        }
                        else
                        {
                            var navParam = new ShellNavigationQueryParameters
                            {
                                 {"Character", character }
                            };
                                await Shell.Current.GoToAsync($"///CharacterPage", navParam);
                            
                        }
                    }
                    else if (check.Id.Value > 800)
                    {                  
                       Vehicle? vehicle = Vehicle.Vehicles.FirstOrDefault(m => m.Id == check.Id);
                        if (vehicle == null) 
                        {
                           await DisplayAlert("Oops...", "Something went wrong with vehicle.", "Ok.");
                        }
                        else
                        {
                            var navParam = new ShellNavigationQueryParameters
                            {
                                 {"Vehicle", vehicle }
                            };
                                await Shell.Current.GoToAsync($"///VehiclePage", navParam);
                        }
                    }
                    else
                    {
                        await DisplayAlert("Oops...", "Something went wrong with Id.", "Ok.");
                    }
                    
                }               

            }
        }
    }

}