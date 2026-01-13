using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DocumentStorage.Client.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace DocumentStorage.Client.Services
{
    public class DocumentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7000"; // Update with your Server API URL

        public DocumentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<DocumentDto>> GetDocumentsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/documents");
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
                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10MB max
                using var streamContent = new StreamContent(fileStream);

                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(streamContent, "file", file.Name);

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/documents/upload", content);

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

                return new UploadResult
                {
                    Success = false,
                    Message = "Upload failed. Please try again."
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
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/documents/{id}");
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
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/documents/{id}/download");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}