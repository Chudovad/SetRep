using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SetRep.MiniApp.Client.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddApplicationServices(builder);

await builder.Build().RunAsync();
