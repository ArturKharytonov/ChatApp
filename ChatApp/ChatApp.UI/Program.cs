using Blazored.LocalStorage;
using ChatApp.Application.AuthenticationService;
using ChatApp.Application.AuthenticationService.Interfaces;
using ChatApp.Application.CustomAuthenticationState;
using ChatApp.Application.HttpClientPWAService;
using ChatApp.Application.HttpClientPWAService.Interfaces;
using ChatApp.Application.RoomApplicationService;
using ChatApp.Application.RoomApplicationService.Interfaces;
using ChatApp.Application.UserApplicationService;
using ChatApp.Application.UserApplicationService.Interfaces;
using ChatApp.UI;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddRadzenComponents();

builder.Services.AddScoped<IRoomApplicationService, RoomApplicationService>();
builder.Services.AddScoped<IUserApplicationService, UserApplicationService>();
builder.Services.AddScoped<IHttpClientPwa, HttpClientPwa>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

await builder.Build().RunAsync();
