using CommunityToolkit.Maui;
using DimensionalTag.Tools;
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
            builder.Services.AddSingleton<Settings>();
            builder.Services.AddSingleton<PortalViewModel>();
            builder.Services.AddSingleton<PortalPage>();
            builder.Services.AddSingleton<LoadingViewModel>();
            builder.Services.AddSingleton<Loading>();
            builder.Services.AddSingleton<CharacterViewModel>();
            builder.Services.AddSingleton<CharacterPage>();
            builder.Services.AddSingleton<VehicleViewModel>();
            builder.Services.AddSingleton<VehiclesPage>();
            builder.Services.AddSingleton<WorldsViewModel>();
            builder.Services.AddSingleton<WorldsPage>();
            builder.Services.AddSingleton<ScanViewModel>();
            builder.Services.AddSingleton<ScanPage>();
            builder.Services.AddTransient<SearchPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
