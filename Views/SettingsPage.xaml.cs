using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;


namespace DimensionalTag;

public partial class SettingsPage : ContentPage
{
    public SettingsViewModel ViewModel { get; set; }
    public SettingsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        ViewModel = vm;
        BindingContext = vm;
        this.Loaded += Page_Loaded;
    }

    void Page_Loaded(object? sender, EventArgs e)
    {
        //Only need to fire this once then we can forget it.
        this.Loaded -= Page_Loaded;
    }

    async Task Close()
    {
        //Nav away without the default animation
        await Navigation.PopModalAsync(animated: false);
    }


    private async void BtnCancel_Clicked(object sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        await Close();
    }

    private async void Tapped_Outside(object sender, TappedEventArgs e)
    {
        await Close();
    }

    private void BgmSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        Preferences.Default.Set("Bgm", e.NewValue);
    }

    private void SfxSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        Preferences.Default.Set("Sfx", e.NewValue);
    }

    private void SkipSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        Preferences.Default.Set("save", e.Value);        
    }

    private void WriteSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        Preferences.Default.Set("WritingType", e.Value);
    }

}