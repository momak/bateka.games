using bateka.games;
using bateka.games.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Same shape as fit: each concern registers itself.
builder.Services
    .AddGamesUi(builder.HostEnvironment.BaseAddress);

await builder.Build().RunAsync();
