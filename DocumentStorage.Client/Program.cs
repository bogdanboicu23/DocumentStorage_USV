using DocumentStorage.Client.Components;
using DocumentStorage.Client.Services;
using DocumentStorage.Client.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Authentication services
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "Cookies";
    })
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

// Add Authorization
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Add custom authentication state provider for server-side Blazor
// IMPORTANT: ServerAuthStateProvider must be singleton to persist across the circuit
builder.Services.AddSingleton<ServerAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
{
    var singleton = provider.GetRequiredService<ServerAuthStateProvider>();
    return singleton;
});

// Add HttpClient and Services
// Configure HttpClient to handle SSL certificates in development
builder.Services.AddHttpClient("AuthorizedClient", client =>
{
    // Base configuration can be added here if needed
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    if (builder.Environment.IsDevelopment())
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };
    }
    return new HttpClientHandler();
});

builder.Services.AddScoped<HttpClient>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("AuthorizedClient");

    // Try to get the current token and set it
    var authProvider = sp.GetService<ServerAuthStateProvider>();
    if (authProvider != null)
    {
        var token = authProvider.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    return httpClient;
});

// DocumentService needs access to ServerAuthStateProvider and uses the factory
builder.Services.AddScoped<DocumentService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var authStateProvider = provider.GetRequiredService<ServerAuthStateProvider>();
    var logger = provider.GetRequiredService<ILogger<DocumentService>>();
    return new DocumentService(httpClientFactory, authStateProvider, logger);
});
// AuthService needs access to the singleton ServerAuthStateProvider
builder.Services.AddScoped<AuthService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var authStateProvider = provider.GetRequiredService<ServerAuthStateProvider>();
    var jsRuntime = provider.GetRequiredService<IJSRuntime>();
    var logger = provider.GetRequiredService<ILogger<AuthService>>();
    return new AuthService(httpClientFactory, authStateProvider, jsRuntime, logger);
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

app.UseStaticFiles();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();