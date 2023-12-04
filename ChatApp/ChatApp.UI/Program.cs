using Blazored.LocalStorage;
using ChatApp.UI;
using ChatApp.UI.Services.CustomAuthenticationState;
using ChatApp.UI.Services.HttpClientPWAService;
using ChatApp.UI.Services.HttpClientPWAService.Interfaces;
using ChatApp.UI.Services.MessageApplicationService;
using ChatApp.UI.Services.MessageApplicationService.Interfaces;
using ChatApp.UI.Services.RoomApplicationService;
using ChatApp.UI.Services.RoomApplicationService.Interfaces;
using ChatApp.UI.Services.RtcService;
using ChatApp.UI.Services.RtcService.Interfaces;
using ChatApp.UI.Services.SignalRService;
using ChatApp.UI.Services.SignalRService.Interfaces;
using ChatApp.UI.Services.UserApplicationService;
using ChatApp.UI.Services.UserApplicationService.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using AuthenticationService = ChatApp.UI.Services.AuthenticationService.AuthenticationService;
using IAuthenticationService = ChatApp.UI.Services.AuthenticationService.Interfaces.IAuthenticationService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddRadzenComponents();
builder.Services.AddBlazorContextMenu();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<DialogService>();

builder.Services.AddScoped<ISignalRService, SignalRService>();
builder.Services.AddScoped<IWebRtcService, WebRtcService>();

builder.Services.AddScoped<IMessageApplicationService, MessageApplicationService>();
builder.Services.AddScoped<IRoomApplicationService, RoomApplicationService>();
builder.Services.AddScoped<IUserApplicationService, UserApplicationService>();
builder.Services.AddScoped<IHttpClientPwa, HttpClientPwa>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

if (builder.HostEnvironment.IsDevelopment())
    Environment.SetEnvironmentVariable("API_URL", "https://localhost:7223");

else if (builder.HostEnvironment.IsProduction())
    Environment.SetEnvironmentVariable("API_URL", "https://apichatappkh.azurewebsites.net");

await builder.Build().RunAsync();
