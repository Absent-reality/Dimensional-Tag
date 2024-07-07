using CommunityToolkit.Maui.Views;

namespace DimensionalTag;

[QueryProperty(nameof(CharacterParam), nameof(CharacterParam))]

public partial class CharacterPage : ContentPage
{
    public Character CharacterParam
    {
        set { SpinTo(value); }
    }

    public CharacterPage()
	{
		InitializeComponent();
        carousel.ItemsSource = Character.Characters;
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
        var width = (DeviceDisplay.MainDisplayInfo.Width)/2;

        await Task.Delay(500);
        await char_title.TranslateTo(-width, 0, 100);
        await char_title.FadeTo(1);
        await char_title.TranslateTo(0, 0, 250);

        await Task.Delay(500);
        await carousel.FadeTo(1);

    }

    private async void SpinTo(Character character)
    {
        await Task.Delay(800);
        var check = Character.Characters.FirstOrDefault(x => x.Name == character.Name);
        if (check != null)
        {
            int start = carousel.Position;
            var number = Character.Characters.IndexOf(check);

            if (carousel.Position == number)
            {
                carousel.Position = number;
            }
            else if (start < number)
            {
                for (int idc = 0; idc < number - start; idc++)
                {
                    await Task.Delay(250);
                    carousel.Position++;
                }
            }
            else if (start > number)
            {
                for (int idc = start; idc > number; idc--)
                {
                    await Task.Delay(250);
                    carousel.Position--;
                }
            }
        }
    }

    private void OnArrival(object sender, NavigatedToEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = true;
        PoppingIn();
    }

    private void OnGoodbye(object sender, NavigatedFromEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = false;
        char_title.FadeTo(0);
        carousel.FadeTo(0);
    }

    private async void Character_Tapped(object sender, TappedEventArgs e)
    {
#if ANDROID
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

    }
}