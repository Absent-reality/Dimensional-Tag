using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;


namespace DimensionalTag;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
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


    private async void BtnCancel_Clicked(object sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        await Close();
    }

    private void On_Arrived(object sender, NavigatedToEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = true;
    }

    private async void Tapped_Outside(object sender, TappedEventArgs e)
    {
        await Close();
    }

    private void On_Goodbye(object sender, NavigatedFromEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = false;
    }

    private void BgmSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var bgm = sender as Slider;
        Preferences.Default.Set<double>("Bgm", bgm!.Value);
    }

    private void SfxSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var sfx = sender as Slider;
        Preferences.Default.Set<double>("Sfx", sfx!.Value);
    }

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        var save = sender as Switch;
        Preferences.Default.Set("save", save!.IsToggled); 
              
    }

}