using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TimerTesting.Gui;
using TimerTesting.Logic;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<Clock>();
builder.Services.AddKeyedTransient("WithOffset", (p, k) => new Clock(new OffsetTimeProvider()));
builder.Services.AddSingleton(TimeProvider.System);

await builder.Build().RunAsync();
