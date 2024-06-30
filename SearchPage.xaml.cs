using System.Collections.ObjectModel;

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

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {

		string test = e.NewTextValue;
        ObservableCollection<string> items = [];
        if (test != "")
        {
            items = LegoTag.SearchTags(test);
            searchResults.ItemsSource = items;          
        }
        else { searchResults.ItemsSource = items;  }
            
    }

    private async void btnCancel_Clicked(object sender, EventArgs e)
    {
        await Close();
    }

    private void On_Arrived(object sender, NavigatedToEventArgs e)
    {
        searchBar.Text = string.Empty;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Close();
    }
}