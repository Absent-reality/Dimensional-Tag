using CommunityToolkit.Mvvm.Input;
using DimensionalTag.Tools;

namespace DimensionalTag
{
    public partial class AppShell : Shell
    {
        public SettingsViewModel SettingsVM { get; private set; }
        public SettingsPage SettingsPage { get; private set; }
        public SearchViewModel SearchVM { get; private set; }
        public Settings? Settings;

        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;
            SettingsVM = new SettingsViewModel();
            SettingsPage = new SettingsPage(SettingsVM);
            SearchVM = new SearchViewModel();
            Settings = Settings.GetInstance();

            if (Settings.NfcEnabled)
            {
                Tabby.Items.Add(new Tab()
                {
                    Title = "Scan",
                    Icon = "scan_ico.png",
                    Items = {  new ShellContent()  { Route = "ScanPage",
                               ContentTemplate = new DataTemplate(() => new ScanPage(new ScanViewModel())) }
                           }
                });

                CenterTab.Items.Add( new ShellContent() 
                { 
                      Route = "PortalPage",
                      ContentTemplate = new DataTemplate(() => 
                                    new PortalPage(new PortalViewModel()))   
                });
                                                    
            }
            else
            {
                Tabby.Items.Add(new Tab()
                {
                    Title = "Portal",
                    Icon = "scan_ico.png",
                    Items = {  new ShellContent()  { Route = "PortalPage",
                               ContentTemplate = new DataTemplate(() => new PortalPage(new PortalViewModel())) }
                           }
                });

                CenterTab.Items.Add(new ShellContent() { Icon = "placeholder.png", IsEnabled = false });
                CenterTab.IsEnabled = false;              
            }

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
         async Task Search_Tapped()
        {
            await Current.Navigation.PushModalAsync(new SearchPage(SearchVM));
        }

    }
}
