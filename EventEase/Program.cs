using EventEase;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// Event service (uses IJSRuntime internally)
builder.Services.AddScoped<EventEase.Services.IEventService, EventEase.Services.EventService>();
// User session service
builder.Services.AddScoped<EventEase.Services.UserSessionService>();

await builder.Build().RunAsync();
