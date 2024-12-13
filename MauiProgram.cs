using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace DimensionalTag
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .RegisterServices()
                .RegisterViews()
                .RegisterViewModels()
#if ANDROID
                .ConfigureMauiHandlers(handlers =>
                {
                    handlers.AddHandler<Shell, CustomShellHandler>();
                })
#endif
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
                
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        { 
            builder.Services.AddSingleton<AppSettings>();
            builder.Services.AddTransient<IAlert, MauiAlert>();
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<App>();
#if ANDROID
            builder.Services.AddSingleton<INfcTools, NfcTools>();
            builder.Services.AddSingleton<MainActivity>();
#endif
            return builder;
        }

        public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {           
            builder.Services.AddSingleton<CharacterPage>();
            builder.Services.AddSingleton<Loading>();
            builder.Services.AddSingleton<PortalPage>();
            builder.Services.AddSingleton<ScanPage>();
            builder.Services.AddTransient<SearchPage>();
            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddSingleton<VehiclesPage>();
            builder.Services.AddSingleton<WorldsPage>();
            return builder;
        }

        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<BaseViewModel>();
            builder.Services.AddSingleton<CharacterViewModel>();
            builder.Services.AddSingleton<LoadingViewModel>();
            builder.Services.AddSingleton<PortalViewModel>();
            builder.Services.AddSingleton<ScanViewModel>();
            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<VehicleViewModel>();
            builder.Services.AddSingleton<WorldsViewModel>();
            return builder;
        }
    }
}
