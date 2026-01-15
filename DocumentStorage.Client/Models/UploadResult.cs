using DocumentStorage.Shared.DTOs.Usage;

namespace DocumentStorage.Client.Models
{
    public class UploadResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public DocumentDto? Document { get; set; }
        public bool IsLimitExceeded { get; set; }
        public UsageDto? CurrentUsage { get; set; }
    }
}