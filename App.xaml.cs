using CommunityToolkit.Maui.Views;
using DimensionalTag.Tools;

namespace DimensionalTag
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Dark;        
            MainPage = new AppShell();

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
