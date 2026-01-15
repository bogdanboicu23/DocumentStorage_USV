using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DocumentStorage.Client.Models;
using Microsoft.AspNetCore.Components.Forms;
using DocumentStorage.Client.Services.Auth;
using Microsoft.Extensions.Logging;
using DocumentStorage.Shared.DTOs.Usage;

namespace DocumentStorage.Client.Services
{
    public class DocumentService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ServerAuthStateProvider _authStateProvider;
        private readonly ILogger<DocumentService> _logger;
        private readonly string _baseUrl = "https://localhost:7150"; // API URL

        public DocumentService(IHttpClientFactory httpClientFactory, ServerAuthStateProvider authStateProvider, ILogger<DocumentService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _authStateProvider = authStateProvider;
            _logger = logger;
        }

        public async Task<List<DocumentDto>> GetDocumentsAsync()
        {
            try
            {
                var httpClient = CreateAuthorizedHttpClient();
                var response = await httpClient.GetAsync($"{_baseUrl}/api/documents");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<DocumentDto>>() ?? new List<DocumentDto>();
                }
                return new List<DocumentDto>();
            }
            catch
            {
                return new List<DocumentDto>();
            }
        }

        public async Task<UploadResult> UploadDocumentAsync(IBrowserFile file)
        {
            try
            {
                var httpClient = CreateAuthorizedHttpClient();

                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10MB max
                using var streamContent = new StreamContent(fileStream);

                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(streamContent, "file", file.Name);

                // Log the authorization header
                _logger.LogInformation($"Authorization header: {httpClient.DefaultRequestHeaders.Authorization}");

                var response = await httpClient.PostAsync($"{_baseUrl}/api/documents/upload", content);

                if (response.IsSuccessStatusCode)
                {
                    var document = await response.Content.ReadFromJsonAsync<DocumentDto>();
                    return new UploadResult
                    {
                        Success = true,
                        Document = document,
                        Message = "File uploaded successfully!"
                    };
                }

                // Check if it's a limit exceeded error (403 Forbidden)
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    try
                    {
                        var limitError = await response.Content.ReadFromJsonAsync<LimitExceededResponse>();
                        if (limitError?.Error == "LIMIT_EXCEEDED")
                        {
                            return new UploadResult
                            {
                                Success = false,
                                Message = limitError.Message ?? "Storage limit exceeded",
                                IsLimitExceeded = true,
                                CurrentUsage = limitError.CurrentUsage
                            };
                        }
                    }
                    catch(Exception ex)
                    {
                       throw new Exception(ex.Message);
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();

                return new UploadResult
                {
                    Success = false,
                    Message = $"Upload failed: {response.StatusCode}. {errorContent}"
                };
            }
            catch (Exception ex)
            {
                return new UploadResult
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<bool> DeleteDocumentAsync(Guid id)
        {
            try
            {
                var httpClient = CreateAuthorizedHttpClient();
                var response = await httpClient.DeleteAsync($"{_baseUrl}/api/documents/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<byte[]?> DownloadDocumentAsync(Guid id)
        {
            try
            {
                var httpClient = CreateAuthorizedHttpClient();
                var response = await httpClient.GetAsync($"{_baseUrl}/api/documents/{id}/download");
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Successfully downloaded document {id}");
                    return await response.Content.ReadAsByteArrayAsync();
                }
                _logger.LogWarning($"Failed to download document {id}: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogError($"Document {id} not found on server");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogError($"Unauthorized access when downloading document {id}");
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading document {id}");
                return null;
            }
        }

        private HttpClient CreateAuthorizedHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient("AuthorizedClient");
            var token = _authStateProvider.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                _logger.LogInformation($"Authorization header set with token");
            }
            else
            {
                _logger.LogWarning("No auth token available");
            }
            return httpClient;
        }

        private class LimitExceededResponse
        {
            public string? Error { get; set; }
            public string? Message { get; set; }
            public UsageDto? CurrentUsage { get; set; }
            public bool UpgradeRequired { get; set; }
        }
    }
}