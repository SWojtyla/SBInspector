using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using SEBInspector.Maui.Application.Services;
using SEBInspector.Maui.Core.Domain;
using SEBInspector.Maui.Core.Interfaces;
using SEBInspector.Maui.Infrastructure.ServiceBus;
using SEBInspector.Maui.Infrastructure.Storage;
using SEBInspector.Maui.Services;

namespace SEBInspector.Maui
{
    public static class MauiProgram
    {
        private sealed class SimpleHostEnvironment : IHostEnvironment
        {
            public SimpleHostEnvironment(string contentRootPath)
            {
                ContentRootPath = contentRootPath;
                ContentRootFileProvider = new NullFileProvider();
            }

            public string ApplicationName { get; set; } = "SBInspector";
            public string EnvironmentName { get; set; } = Environments.Production;
            public string ContentRootPath { get; set; }
            public IFileProvider ContentRootFileProvider { get; set; }
        }

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

            // Add Data Protection without relying on MAUI's host environment implementation.
            var dataProtectionKeyPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SBInspector",
                "Keys");
            Directory.CreateDirectory(dataProtectionKeyPath);
            builder.Services.AddSingleton<IHostEnvironment>(new SimpleHostEnvironment(AppContext.BaseDirectory));
            builder.Services
                .AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeyPath))
                .SetApplicationName("SBInspector");

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
