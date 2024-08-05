using CommunityToolkit.Maui.Alerts;
using System.Windows.Input;

namespace DimensionalTag
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public ICommand CenterViewCommand { get; } = new Command(async () => await Current.Navigation.PushModalAsync(new SearchPage()));

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            var vm = Current.CurrentPage.BindingContext as SettingsViewModel;
            await vm!.ShowIt();
        }

    }
}
