using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SBInspector.Core.Interfaces;
using SBInspector.Infrastructure.ServiceBus;
using SBInspector.Infrastructure.Storage;
using SBInspector.Application.Services;
using SBInspector.Presentation.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register services following clean architecture
builder.Services.AddSingleton<IServiceBusService, ServiceBusService>();
builder.Services.AddSingleton<MessageFilterService>();

// Register storage configuration service
builder.Services.AddSingleton<StorageConfigurationService>();

// Register storage service with factory pattern
builder.Services.AddScoped<IStorageService>(sp =>
{
    var jsRuntime = sp.GetRequiredService<Microsoft.JSInterop.IJSRuntime>();
    var configService = sp.GetRequiredService<StorageConfigurationService>();
    var configuration = configService.GetConfiguration();
    var factory = new StorageServiceFactory(jsRuntime, configuration);
    return factory.CreateStorageService();
});

await builder.Build().RunAsync();
