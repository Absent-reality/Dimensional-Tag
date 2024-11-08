using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm;
using System.Collections.ObjectModel;

namespace DimensionalTag;

[QueryProperty(nameof(CharacterParam), nameof(CharacterParam))]

public partial class CharacterPage : ContentPage
{
    public Character CharacterParam
    {
        set { SpinTo(value); }
    }

    public CharacterViewModel Vm {  get; set; }
    private int _firstIndex = 0;
    public int FirstIndex 
    {
        get { return _firstIndex; }
        set
        {
            if (_firstIndex == value)
                return;
            _firstIndex = value;
            OnPropertyChanged(nameof(FirstIndex));          
        }
    }

    public int LastIndex
    {
        get { return Vm.LastIndex; }
        set
        {
            if (Vm.LastIndex == value)
                return;
            Vm.LastIndex = value;
            OnPropertyChanged(nameof(LastIndex));
        }
    }

    public int CenterIndex
    {
        get { return Vm.CenterIndex; }
        set
        {
            if (Vm.CenterIndex == value)
                return;
            Vm.CenterIndex = value;
            OnPropertyChanged(nameof(CenterIndex));
            OnPosition_Changed();
        }
    }

    public CharacterPage(CharacterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        Vm = vm;
        vm.cv = collection;
        vm.GetList();
        LastIndex = vm.AllCharacters.Count - 1;

        sfx.Source = MediaSource.FromResource("swish.mp3");
        bgm.Source = MediaSource.FromResource("Defender_Main.mp3");

        var window = App.Window;
        window.Destroying += (s, e) =>
        {
            bgm.Handler?.DisconnectHandler();
            sfx.Handler?.DisconnectHandler();
        };

        this.Loaded += Page_Loaded;
        
    }

    void Page_Loaded(object? sender, EventArgs e)
    {
        this.Loaded -= Page_Loaded;      
        bgm.Volume = Vm.CheckValue("Bgm", bgm.Volume);
        sfx.Volume = Vm.CheckValue("Sfx", sfx.Volume);
        collection.ScrollTo(1);
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
        char_title.Shadow = new Shadow
        {
            Brush = Colors.DimGray,
            Offset = new(5, 5),
            Radius = 5,
            Opacity = .8f
        };

        sfx.Play();
        await Task.Delay(500);
        sfx.Stop();

        await Task.Delay(500);
        View[] views = [];
        if (FirstIndex != 0 && LastIndex != Vm.AllCharacters.Count - 1)
        { views = [collection, rightArrow, leftArrow]; }
        else if (FirstIndex == 0) { views = [collection, rightArrow]; }
        else if (LastIndex == Vm.AllCharacters.Count - 1) { views = [collection, leftArrow]; }
        await FadeGroup(views, 1);

    }

    private void OnArrival(object sender, NavigatedToEventArgs e)
    {
        PoppingIn();        
    }

    private async void OnGoodbye(object sender, NavigatedFromEventArgs e)
    {
        if (sfx.CurrentState == MediaElementState.Playing)
        {
            sfx.Stop();
        }
        
        await FadeGroup([char_title, collection, rightArrow, leftArrow], 0);
    }

    private void OnPosition_Changed() 
    {
        if (sfx.CurrentState == MediaElementState.Playing)
        {
            sfx.Stop();
        }
        sfx.Source = MediaSource.FromResource("click.mp3");
        sfx.Play();           
    }

    private void OnScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        var source = collection.ItemsSource as ObservableCollection<Character>;
        if (source == null) { return; }
        var total = source.Count;

        if (e.FirstVisibleItemIndex == 0) { leftArrow.Opacity = 0; }
        else
        {  
            leftArrow.Opacity = 1;
            leftArrow.Shadow = new Shadow
            {
                Brush = Colors.DimGray,
                Offset = new(-5, 5),
                Radius = 5,
                Opacity = .8f
            };
        }

        if (e.LastVisibleItemIndex == total - 1) { rightArrow.Opacity = 0; }
        else { rightArrow.Opacity = 1; }

        FirstIndex = e.FirstVisibleItemIndex;
        LastIndex = e.LastVisibleItemIndex;
        CenterIndex = e.CenterItemIndex;
        collection.SelectedItem = source[CenterIndex];
    }

    public void SpinTo(Character character)
    {
        collection.ScrollTo(Vm.GetCharacterPosition(character));
    }

    private void Arrow_Tapped(object sender, TappedEventArgs e)
    {
        var img = (Image)sender;
        switch (img.AutomationId)
        {
            case "Left":
                collection.ScrollTo(0);
                break;

            case "Right":
                collection.ScrollTo(Vm.AllCharacters.Count -1);
                break;             
        }       
    }

    public async Task FadeGroup(View[] views, double opacity)
    {
        if (views == null)
        {
            ArgumentNullException.ThrowIfNull(nameof(views)); 
            return;
        }
        if (views.Length != 0)
        {
            foreach (var v in views)
            {
                await v.FadeTo(opacity);
            }
        }
    }
}