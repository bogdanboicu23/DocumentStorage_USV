namespace DocumentStorage.Client.Models
{
    public class UploadResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public DocumentDto? Document { get; set; }
    }
}