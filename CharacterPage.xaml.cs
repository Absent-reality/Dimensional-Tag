#if ANDROID
using Android.App;
using Android.Nfc.Tech;
using Android.OS;
using DimensionalTag.Enums;
using DimensionalTag.Interfaces;
using Java.Util.Concurrent;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Text;
#endif

namespace DimensionalTag;

public partial class CharacterPage : ContentPage
{
	public CharacterPage()
	{
		InitializeComponent();
        var Char = Character.Characters;
        carousel.ItemsSource = Char;

    }

    private void OnArrival(object sender, NavigatedToEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = true;

    }

    private void OnGoodbye(object sender, NavigatedFromEventArgs e)
    {
        DeviceDisplay.KeepScreenOn = false;
    }

}