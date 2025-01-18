#if ANDROID
using Android.Content;
using Android.Nfc;
#endif
using CommunityToolkit.Mvvm.Input;

namespace DimensionalTag
{
    public partial class AppShell : Shell
    {

        public SettingsViewModel SettingsVM { get; private set; }
        public SettingsPage SettingsPage { get; private set; }
        public SearchViewModel SearchVM { get; private set; }
        public AppSettings Settings;

        public AppShell(INfcTools nfcTools, AppSettings settings, IAlert alert)
        {
            InitializeComponent();
            BindingContext = this;             
            SettingsVM = new SettingsViewModel(settings, alert);
            SettingsPage = new SettingsPage(SettingsVM);
            SearchVM = new SearchViewModel(settings, alert);
            Settings = settings;

#if ANDROID

            if (Android.App.Application.Context.GetSystemService(Context.NfcService) is not NfcManager manager) { return; }
            var adapter = manager.DefaultAdapter;  
                      
            if (adapter != null )
            {
                Tabby.Items.Add(new Tab()
                {
                    Title = "Scan",
                    Icon = "scan_ico.png",
                    Items = {  new ShellContent()  { Route = "ScanPage",
                               ContentTemplate = new DataTemplate(() => new ScanPage(new ScanViewModel(settings, alert, nfcTools), nfcTools)) }
                           }
                });

                CenterTab.Items.Add( new ShellContent() 
                { 
                      Route = "PortalPage",
                      ContentTemplate = new DataTemplate(() => 
                                    new PortalPage(new PortalViewModel(settings, alert)))   
                });
                                                    
            }
            else
            {
                Tabby.Items.Add(new Tab()
                {
                    Title = "Portal",
                    Icon = "portal_ico.png",
                    Items = {  new ShellContent()  { Route = "PortalPage",
                               ContentTemplate = new DataTemplate(() => new PortalPage(new PortalViewModel(settings, alert))) }
                           }
                });

                CenterTab.Items.Add(new ShellContent() { Icon = "placeholder.png", IsEnabled = false });
                CenterTab.IsEnabled = false; 
                Img_Port.IsVisible = false;
            }
#endif
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
