using Microsoft.JSInterop;
using MudBlazor.Services;
using SetRep.MiniApp.Client.Services;
using SetRep.MiniApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Регистрация сервисов для работы с Telegram WebApp (для prerendering)
// На сервере эти сервисы будут использоваться только при prerendering
// В WebAssembly они будут переопределены регистрацией из Client/Program.cs
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7097";
builder.Services.AddScoped<ITelegramWebAppService>(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
    var jsRuntime = sp.GetService<IJSRuntime>() ?? throw new InvalidOperationException("IJSRuntime is not available");
    var logger = sp.GetRequiredService<ILogger<TelegramWebAppService>>();
    return new TelegramWebAppService(jsRuntime, httpClient, logger);
});
builder.Services.AddSingleton<ITelegramWebAppStateService, TelegramWebAppStateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SetRep.MiniApp.Client._Imports).Assembly);

app.Run();
