using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using SBInspector.Shared.Application.Services;
using SBInspector.Shared.Core.Domain;
using SBInspector.Shared.Core.Interfaces;
using SBInspector.Shared.Infrastructure.ServiceBus;
using SBInspector.Shared.Infrastructure.Storage;
using SEBInspector.Maui.Services;

namespace SEBInspector.Maui
{
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

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddMudServices();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Add Data Protection for secure storage
            builder.Services.AddDataProtection()
                .SetApplicationName("SBInspector")
                .PersistKeysToFileSystem(new DirectoryInfo(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SBInspector", "Keys")));

            // Register services following clean architecture
            builder.Services.AddSingleton<IServiceBusService, ServiceBusService>();
            builder.Services.AddSingleton<MessageFilterService>();
            builder.Services.AddSingleton<ConnectionStateService>();
            builder.Services.AddSingleton<ConnectionStringEncryptionService>();
            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<ColumnConfigurationService>();

            // Register storage configuration service with FileSystem as default for MAUI
            builder.Services.AddSingleton<StorageConfigurationService>(sp => 
                new StorageConfigurationService(StorageType.FileSystem));

            // Register storage service with factory pattern
            builder.Services.AddScoped<IStorageService>(sp =>
            {
                var jsRuntime = sp.GetRequiredService<Microsoft.JSInterop.IJSRuntime>();
                var configService = sp.GetRequiredService<StorageConfigurationService>();
                var configuration = configService.GetConfiguration();
                var factory = new StorageServiceFactory(jsRuntime, configuration);
                return factory.CreateStorageService();
            });

            // Register file export service for MAUI
            builder.Services.AddSingleton<IFileExportService, MauiFileExportService>();

            // Register folder picker service for MAUI
            builder.Services.AddSingleton<IFolderPickerService, MauiFolderPickerService>();

            return builder.Build();
        }
    }
}
