using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;

namespace DimensionalTag;

[QueryProperty(nameof(WorldParam), nameof(WorldParam))]

public partial class WorldsPage : ContentPage
{
   
    public World WorldParam
    {
        set { SpinTo(value); }
    }

    public WorldsViewModel Vm { get; set; }
    private int _worldFirstIndex = 0;
    public int WorldFirstIndex
    {
        get { return _worldFirstIndex; }
        set
        {
            if (_worldFirstIndex == value)
                return;
            _worldFirstIndex = value;
            OnPropertyChanged(nameof(WorldFirstIndex));
        }
    }

    public int WorldCenterIndex
    {
        get { return Vm.WorldCenterIndex; }
        set
        {
            if (Vm.WorldCenterIndex == value)
                return;
            Vm.WorldCenterIndex = value;
            OnPropertyChanged(nameof(WorldCenterIndex));
            OnPosition_Changed();
        }
    }

    public int WorldLastIndex
    {
        get { return Vm.WorldLastIndex; }
        set
        {
            if (Vm.WorldLastIndex == value)
                return;
            Vm.WorldLastIndex = value;
            OnPropertyChanged(nameof(WorldLastIndex));
        }
    }

    private int _itemFirstIndex = 0;
    public int ItemFirstIndex
    {
        get { return _itemFirstIndex; }
        set
        {
            if (_itemFirstIndex == value)
                return;
            _itemFirstIndex = value;
            OnPropertyChanged(nameof(ItemFirstIndex));
        }
    }

    public int ItemCenterIndex
    {
        get { return Vm.ItemCenterIndex; }
        set
        {
            if (Vm.ItemCenterIndex == value)
                return;
            Vm.ItemCenterIndex = value;
            OnPropertyChanged(nameof(ItemCenterIndex));
            OnPosition_Changed();
        }
    }

    public int ItemLastIndex
    {
        get { return Vm.ItemLastIndex; }
        set
        {
            if (Vm.ItemLastIndex == value)
                return;
            Vm.ItemLastIndex = value;
            OnPropertyChanged(nameof(ItemLastIndex));
        }
    }

    public WorldsPage(WorldsViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        Vm = vm;
        vm.cv = worldCollection;
        vm.GetWorldList();
        WorldLastIndex = vm.AllWorlds.Count -1;

        sfx.Source = MediaSource.FromResource("swish.mp3");
      //  World_Carousel.ItemsSource = World.Worlds;
        this.Loaded += Page_Loaded;
    }

    void Page_Loaded(object? sender, EventArgs e)
    {
        this.Loaded -= Page_Loaded;
        sfx.Volume = Vm.CheckValue("Sfx", sfx.Volume);
        worldCollection.ScrollTo(1);
    }

    public async void PoppingIn()
    {
        sfx.Source = MediaSource.FromResource("swish.mp3");
        //measure the display size to know how far to translate.
        var width = (DeviceDisplay.MainDisplayInfo.Width) / 2;

        await Task.Delay(500);
        await world_title.TranslateTo(-width, 0, 100);
        await world_title.FadeTo(1);
        world_title.IsVisible = true;
        await world_title.TranslateTo(0, 0, 250);
        world_title.Shadow = new Shadow
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
        if (ItemFirstIndex != 0 && ItemLastIndex != Vm.SortedItems.Count - 1)
        { views = [worldCollection, itemCollection, rightArrow, leftArrow]; }
        else if (ItemFirstIndex == 0) { views = [worldCollection, itemCollection, rightArrow]; }
        else if (ItemLastIndex == Vm.SortedItems.Count - 1) { views = [worldCollection, itemCollection, leftArrow]; }
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

        await FadeGroup([world_title, worldCollection, itemCollection, rightArrow, leftArrow], 0);
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

    private void On_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        var obj = (CollectionView)sender;

        switch (obj.AutomationId)
        {
            case "Worlds":

                var worldSource = worldCollection.ItemsSource as ObservableCollection<World>;
                if (worldSource == null) { return; }

                WorldFirstIndex = e.FirstVisibleItemIndex;
                WorldLastIndex = e.LastVisibleItemIndex;
                WorldCenterIndex = e.CenterItemIndex;
                worldCollection.SelectedItem = worldSource[WorldCenterIndex];

                break;

            case "Items":

                var itemSource = itemCollection.ItemsSource as ObservableCollection<SearchItems>;
                if (itemSource == null) { return; }
                var total = itemSource.Count;

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

                ItemFirstIndex = e.FirstVisibleItemIndex;
                ItemLastIndex = e.LastVisibleItemIndex;
                ItemCenterIndex = e.CenterItemIndex;
                itemCollection.SelectedItem = itemSource[WorldCenterIndex];
                break;
        }
        
    }

    public void SpinTo(World world)
    {
        worldCollection.ScrollTo(Vm.GetWorldPosition(world), position: ScrollToPosition.Center);
    }

    private void Arrow_Tapped(object sender, TappedEventArgs e)
    {
        var img = (Image)sender;
        switch (img.AutomationId)
        {
            case "Left":
                itemCollection.ScrollTo(1);
                break;

            case "Right":
                itemCollection.ScrollTo(Vm.SortedItems.Count - 2);
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