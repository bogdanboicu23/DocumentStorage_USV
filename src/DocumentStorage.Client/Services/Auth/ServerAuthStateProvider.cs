using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace DocumentStorage.Client.Services.Auth
{
    public class ServerAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILogger<ServerAuthStateProvider> _logger;
        private readonly ConcurrentDictionary<string, string> _userTokens = new();
        private readonly AuthenticationState _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        private string? _currentUserId;

        public ServerAuthStateProvider(ILogger<ServerAuthStateProvider> logger)
        {
            _logger = logger;
            _logger.LogInformation("ServerAuthStateProvider singleton created");
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string? token = null;
            if (!string.IsNullOrEmpty(_currentUserId) && _userTokens.TryGetValue(_currentUserId, out var storedToken))
            {
                token = storedToken;
            }

            _logger.LogInformation("GetAuthenticationStateAsync called, userId: {UserId}, token exists: {TokenExists}",
                _currentUserId, !string.IsNullOrEmpty(token));

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("No token found, returning anonymous");
                return Task.FromResult(_anonymous);
            }

            var claims = ParseClaimsFromJwt(token);
            if (claims == null)
            {
                _logger.LogWarning("Failed to parse claims from token");
                return Task.FromResult(_anonymous);
            }

            var claimsList = claims.ToList();
            var expiry = claimsList.FirstOrDefault(c => c.Type == "exp")?.Value;

            if (expiry != null && IsTokenExpired(expiry))
            {
                _logger.LogInformation("Token expired, removing from storage");
                if (!string.IsNullOrEmpty(_currentUserId))
                {
                    _userTokens.TryRemove(_currentUserId, out _);
                }
                return Task.FromResult(_anonymous);
            }

            var identity = new ClaimsIdentity(claimsList, "jwt");
            var user = new ClaimsPrincipal(identity);
            _logger.LogInformation("Returning authenticated state for user: {UserName}",
                user.Identity?.Name ?? "Unknown");

            return Task.FromResult(new AuthenticationState(user));
        }

        public void NotifyUserAuthentication(string token, HttpClient httpClient)
        {
            _logger.LogInformation("NotifyUserAuthentication called with token");

            var claims = ParseClaimsFromJwt(token);
            if (claims != null)
            {
                var userId = claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    _currentUserId = userId;
                    _userTokens[userId] = token;
                    _logger.LogInformation("Stored token for user: {UserId}", userId);
                }
            }

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            _logger.LogInformation("Notifying authentication state changed, user authenticated: {IsAuthenticated}, userName: {UserName}",
                authenticatedUser.Identity?.IsAuthenticated ?? false,
                authenticatedUser.Identity?.Name ?? "Unknown");

            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout(HttpClient httpClient)
        {
            _logger.LogInformation("NotifyUserLogout called for user: {UserId}", _currentUserId);

            if (!string.IsNullOrEmpty(_currentUserId))
            {
                _userTokens.TryRemove(_currentUserId, out _);
                _currentUserId = null;
            }

            httpClient.DefaultRequestHeaders.Authorization = null;

            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
        }

        public string? GetToken()
        {
            if (!string.IsNullOrEmpty(_currentUserId) && _userTokens.TryGetValue(_currentUserId, out var token))
            {
                return token;
            }
            return null;
        }

        public void SetToken(string? token, HttpClient httpClient)
        {
            _logger.LogInformation("SetToken called, token exists: {TokenExists}", !string.IsNullOrEmpty(token));

            if (!string.IsNullOrEmpty(token))
            {
                var claims = ParseClaimsFromJwt(token);
                if (claims != null)
                {
                    var userId = claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        _currentUserId = userId;
                        _userTokens[userId] = token;
                        _logger.LogInformation("Token set for user: {UserId}", userId);
                    }
                }

                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                _logger.LogInformation("Authorization header set");
            }
            else
            {
                if (!string.IsNullOrEmpty(_currentUserId))
                {
                    _userTokens.TryRemove(_currentUserId, out _);
                    _currentUserId = null;
                }
                httpClient.DefaultRequestHeaders.Authorization = null;
                _logger.LogInformation("Authorization header cleared");
            }
        }

        public void SetCurrentUser(string userId)
        {
            _currentUserId = userId;
            _logger.LogInformation("Current user set to: {UserId}", userId);
        }

        private IEnumerable<Claim>? ParseClaimsFromJwt(string jwt)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                return token.Claims;
            }
            catch
            {
                return null;
            }
        }

        private bool IsTokenExpired(string expiry)
        {
            var expiryDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiry));
            return expiryDateTimeOffset < DateTimeOffset.UtcNow;
        }
    }
}