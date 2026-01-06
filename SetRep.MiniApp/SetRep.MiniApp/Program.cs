using SetRep.MiniApp.Components;
using SetRep.MiniApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder);

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

// Используем HTTPS редирект только если настроен HTTPS порт
var httpsPort = app.Configuration["ASPNETCORE_HTTPS_PORTS"];
if (!string.IsNullOrEmpty(httpsPort))
{
    app.UseHttpsRedirection();
}


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SetRep.MiniApp.Client._Imports).Assembly);

app.Run();
