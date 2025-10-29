using SBInspector.Components;
using SBInspector.Shared.Core.Interfaces;
using SBInspector.Shared.Infrastructure.ServiceBus;
using SBInspector.Shared.Infrastructure.Storage;
using SBInspector.Shared.Infrastructure.Export;
using SBInspector.Shared.Application.Services;
using SBInspector.Shared.Core.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register services following clean architecture
builder.Services.AddSingleton<IServiceBusService, ServiceBusService>();
builder.Services.AddSingleton<MessageFilterService>();

// Register storage configuration service with LocalStorage as default for web
builder.Services.AddSingleton<StorageConfigurationService>(sp => 
    new StorageConfigurationService(StorageType.LocalStorage));

// Register storage service with factory pattern
builder.Services.AddScoped<IStorageService>(sp =>
{
    var jsRuntime = sp.GetRequiredService<Microsoft.JSInterop.IJSRuntime>();
    var configService = sp.GetRequiredService<StorageConfigurationService>();
    var configuration = configService.GetConfiguration();
    var factory = new StorageServiceFactory(jsRuntime, configuration);
    return factory.CreateStorageService();
});

// Register file export service for web
builder.Services.AddScoped<IFileExportService>(sp =>
{
    var jsRuntime = sp.GetRequiredService<Microsoft.JSInterop.IJSRuntime>();
    return new WebFileExportService(jsRuntime);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
