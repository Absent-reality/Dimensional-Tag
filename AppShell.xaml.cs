using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace DimensionalTag
{
    public partial class AppShell : Shell
    {   
        public SettingsViewModel SettingsVM { get; private set; }
        public SettingsPage SettingsPage { get; private set; }
        public SearchViewModel SearchVM { get; private set; }
        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;
            SettingsVM = new SettingsViewModel();
            SettingsPage = new SettingsPage(SettingsVM); 
            SearchVM = new SearchViewModel();
        }

        private async void Settings_Tapped(object sender, TappedEventArgs e)
        {
            if (Img_Set.IsEnabled)
            {
                Img_Set.IsEnabled = false;
                await Navigation.PushModalAsync(SettingsPage);
                Img_Set.IsEnabled = true;
            }
        }

        private async void Portal_Tapped(object sender, TappedEventArgs e)
        {
            if (Img_Port.IsEnabled)
            {
                Img_Port.IsEnabled = false;
                await Shell.Current.GoToAsync($"///PortalPage");
                Img_Port.IsEnabled = true;
            } 
        }

         [RelayCommand]
         async void Search_Tapped()
        {
            await Current.Navigation.PushModalAsync(new SearchPage(SearchVM));
        }

    }
}
