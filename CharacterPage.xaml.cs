
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;

namespace DimensionalTag;

[QueryProperty(nameof(CharacterParam), nameof(CharacterParam))]

public partial class CharacterPage : ContentPage
{
    public Character CharacterParam
    {
        set { SpinTo(value); }
    }

    
    public CharacterPage(SettingsViewModel vm)
	{

		InitializeComponent();

        BindingContext = vm; 
        sfx.BindingContext = vm;
        bgm.BindingContext = vm;

        if (Preferences.Default.ContainsKey("save"))
        {
            bool isSaved = Preferences.Default.Get("save", false);
            if (isSaved) { vm.Save = true; }
            else { vm.Save = false; }
        }
        if (Preferences.Default.ContainsKey("Bgm"))
        {
            double bgmVol = Preferences.Default.Get<double>("Bgm", 0);
            vm.BgmVol = bgmVol;
           
        }
        if (Preferences.Default.ContainsKey("Sfx"))
        {
            double sfxVol = Preferences.Default.Get<double>("Sfx", 0);
            vm.SfxVol = sfxVol;
        }       

        carousel.ItemsSource = Character.Characters;
        sfx.Source = MediaSource.FromResource("swish.mp3");
        bgm.Source = MediaSource.FromResource("Defender_Main.mp3");
        this.Loaded += Page_Loaded;



        var window = App.Window;
        window.Deactivated += (s, e) =>
        {
            bgm.ShouldMute = true;
        };

        window.Activated += (s, e) =>
        {
            bgm.ShouldMute = false;
        };

        window.Destroying += (s, e) =>
        {
            bgm.Handler?.DisconnectHandler();
        };

    }
      
    void Page_Loaded(object? sender, EventArgs e)
    {
        //Only need to fire this once then we can forget it.
        this.Loaded -= Page_Loaded;

        if (Preferences.Default.ContainsKey("Bgm"))
        {
            double bgmVol = Preferences.Default.Get<double>("Bgm", 0);
            bgm.Volume = bgmVol;
        }
        if (Preferences.Default.ContainsKey("Sfx"))
        {
            double sfxVol = Preferences.Default.Get<double>("Sfx", 0);
            sfx.Volume = sfxVol;
        } 

    }

    public async void PoppingIn()
    {
       sfx.Source =  MediaSource.FromResource("swish.mp3");
         //measure the display size to know how far to translate.
        var width = (DeviceDisplay.MainDisplayInfo.Width)/2;

        await Task.Delay(500);
        await char_title.TranslateTo(-width, 0, 100);
        await char_title.FadeTo(1);
        char_title.IsVisible = true;
        await char_title.TranslateTo(0, 0, 250);
        sfx.Play();
        await Task.Delay(500);
        sfx.Stop();

        await Task.Delay(500);
        await carousel.FadeTo(1);
        
    }

    private void SpinTo(Character character)
    {
       
        var check = Character.Characters.FirstOrDefault(x => x.Name == character.Name);
        if (check != null)
        {
            int start = carousel.Position;
            carousel.Position = Character.Characters.IndexOf(check);
        }
    }

    private void OnArrival(object sender, NavigatedToEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = true;
        PoppingIn();        
    }

    private async void OnGoodbye(object sender, NavigatedFromEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = false;
        if (sfx.CurrentState == MediaElementState.Playing)
        {
            sfx.Stop();
        }

        await char_title.FadeTo(0);
        await carousel.FadeTo(0);
 
    }

    private async void Character_Tapped(object sender, TappedEventArgs e)
    {
        carousel.IsEnabled = false;
#if ANDROID
        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        Character? current = carousel.CurrentItem as Character;
        if (current != null) 
        { 
               var popup = new PopupPage(current);
               var result = await Shell.Current.ShowPopupAsync(popup);

            if (result is bool sure)
            {
                var alert = new AlertPopup(" Alert! ", " Are you sure you want to write this data? ", " Cancel?", " Write? ", true);
                var confirm = await Shell.Current.ShowPopupAsync(alert);
                if (confirm is bool tru)
                {
                    var navParam = new Dictionary<string, object> { { "WriteCharacter", current } };

                    await Shell.Current.GoToAsync($"///ScanPage", navParam);
                }

            }
               
        }
#endif        
        carousel.IsEnabled = true;
    }

    private void OnPosition_Changed(object sender, PositionChangedEventArgs e) 
    {
        if (sfx.CurrentState == MediaElementState.Playing)
        {
            sfx.Stop();
        }
        sfx.Source = MediaSource.FromResource("click.mp3");
        sfx.Play();
       
    }

}