using CommunityToolkit.Maui.Views;

namespace DimensionalTag
{
    public partial class App : Application
    {

        public App(INfcTools nfcTools, AppSettings settings, IAlert alert)
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Dark;        
            MainPage = new AppShell(nfcTools, settings, alert);

        }

        public static Window Window { get; private set; }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);
            Window = window;
            return window;

        }

    }
}
