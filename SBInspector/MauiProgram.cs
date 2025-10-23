using Microsoft.Extensions.Logging;
using SBInspector.Presentation.Components;
using SBInspector.Core.Interfaces;
using SBInspector.Infrastructure.ServiceBus;
using SBInspector.Infrastructure.Storage;
using SBInspector.Application.Services;
using SBInspector.Core.Domain;

namespace SBInspector;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Add Blazor WebView services
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Register services following clean architecture
        builder.Services.AddSingleton<IServiceBusService, ServiceBusService>();
        builder.Services.AddSingleton<MessageFilterService>();

        // Register storage configuration service
        builder.Services.AddSingleton<StorageConfigurationService>();

        // Register storage service - for MAUI, we'll use FileStorageService by default
        builder.Services.AddSingleton<IStorageService, FileStorageService>();

        return builder.Build();
    }
}
