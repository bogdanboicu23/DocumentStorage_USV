using System.Net.Http.Json;
using System.Text.Json;
using DocumentStorage.Client.Models.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Net.Http.Headers;
using Microsoft.JSInterop;
using Microsoft.Extensions.Logging;

namespace DocumentStorage.Client.Services.Auth
{
    public class AuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ServerAuthStateProvider _authStateProvider;
        private readonly IJSRuntime _jsRuntime;
        private readonly ILogger<AuthService> _logger;
        private readonly string _baseUrl;

        public AuthService(IHttpClientFactory httpClientFactory, ServerAuthStateProvider authStateProvider, IJSRuntime jsRuntime, ILogger<AuthService> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _authStateProvider = authStateProvider;
            _jsRuntime = jsRuntime;
            _logger = logger;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7150";
        }

        public async Task<bool> LoginAsync(LoginModel loginModel)
        {
            try
            {
                _logger.LogInformation("Attempting login for email: {Email}", loginModel.Email);
                var httpClient = _httpClientFactory.CreateClient("AuthorizedClient");
                var response = await httpClient.PostAsJsonAsync($"{_baseUrl}/api/auth/login", loginModel);

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

                    if (authResponse != null)
                    {
                        // Create a new client to set the header on
                        var authHttpClient = _httpClientFactory.CreateClient("AuthorizedClient");
                        authHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.Token);

                        // Set the token in the provider first
                        _authStateProvider.SetToken(authResponse.Token, authHttpClient);

                        // Notify authentication state changed
                        _authStateProvider.NotifyUserAuthentication(authResponse.Token, authHttpClient);

                        // Give the state time to propagate
                        await Task.Delay(100);

                        return true;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Login failed with status {StatusCode}. Error: {Error}",
                        response.StatusCode, errorContent);
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return false;
            }
        }

        public async Task<(bool Success, string ErrorMessage)> RegisterAsync(RegisterModel registerModel)
        {
            try
            {
                _logger.LogInformation("Starting registration for email: {Email}", registerModel.Email);

                var registerDto = new
                {
                    email = registerModel.Email,
                    password = registerModel.Password,
                    firstName = registerModel.FirstName,
                    lastName = registerModel.LastName
                };

                _logger.LogInformation("Sending registration request to: {Url}", $"{_baseUrl}/api/auth/register");
                var httpClient = _httpClientFactory.CreateClient("AuthorizedClient");
                var response = await httpClient.PostAsJsonAsync($"{_baseUrl}/api/auth/register", registerDto);
                _logger.LogInformation("Registration response status: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Registration successful, reading response");
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

                    if (authResponse != null)
                    {
                        _logger.LogInformation("Auth response received, saving token");

                        // Create a new client to set the header on
                        var authHttpClient = _httpClientFactory.CreateClient("AuthorizedClient");
                        authHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.Token);

                        // Set the token in the provider first
                        _authStateProvider.SetToken(authResponse.Token, authHttpClient);

                        // Notify authentication state changed
                        _authStateProvider.NotifyUserAuthentication(authResponse.Token, authHttpClient);

                        // Give the state time to propagate
                        await Task.Delay(100);

                        return (true, string.Empty);
                    }
                    else
                    {
                        _logger.LogWarning("Auth response was null");
                        return (false, "Invalid response from server");
                    }
                }
                else
                {
                    // Read error response
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Registration failed with status {StatusCode}. Error: {Error}",
                        response.StatusCode, errorContent);

                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent);
                        if (errorResponse.TryGetProperty("message", out var messageElement))
                        {
                            var errorMessage = messageElement.GetString() ?? "Registration failed";
                            _logger.LogWarning("Extracted error message: {ErrorMessage}", errorMessage);

                            if (errorResponse.TryGetProperty("errorType", out var errorTypeElement))
                            {
                                _logger.LogWarning("Error type: {ErrorType}", errorTypeElement.GetString());
                            }

                            return (false, errorMessage);
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, "Failed to parse error response");
                    }

                    return (false, $"Registration failed: {response.StatusCode}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP request failed during registration");

                // Provide user-friendly error messages
                if (httpEx.Message.Contains("Connection refused"))
                {
                    return (false, "Cannot connect to server. Please ensure the API server is running.");
                }
                else if (httpEx.Message.Contains("SSL") || httpEx.Message.Contains("certificate"))
                {
                    return (false, "SSL certificate error. Please check your development certificates.");
                }

                return (false, "Network error. Please check your connection and try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration");
                return (false, "An unexpected error occurred. Please try again later.");
            }
        }

        public async Task LogoutAsync()
        {
            await RemoveTokenAsync();
            var httpClient = _httpClientFactory.CreateClient("AuthorizedClient");
            _authStateProvider.NotifyUserLogout(httpClient);
        }

        private Task SaveTokenAsync(string token)
        {
            // Token is stored in the ServerAuthStateProvider
            var httpClient = _httpClientFactory.CreateClient("AuthorizedClient");
            _authStateProvider.SetToken(token, httpClient);
            return Task.CompletedTask;
        }

        private Task RemoveTokenAsync()
        {
            // Remove token from ServerAuthStateProvider
            var httpClient = _httpClientFactory.CreateClient("AuthorizedClient");
            _authStateProvider.SetToken(null, httpClient);
            return Task.CompletedTask;
        }

        public Task<string?> GetTokenAsync()
        {
            // Get token from ServerAuthStateProvider
            var token = _authStateProvider.GetToken();
            return Task.FromResult(token);
        }
    }
}