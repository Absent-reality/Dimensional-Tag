using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace DimensionalTag
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
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
                
            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddSingleton<CharacterPage>();
            builder.Services.AddSingleton<VehiclesPage>();
            builder.Services.AddSingleton<WorldsPage>();
            builder.Services.AddSingleton<ScanPage>();
            builder.Services.AddTransient<SearchPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
