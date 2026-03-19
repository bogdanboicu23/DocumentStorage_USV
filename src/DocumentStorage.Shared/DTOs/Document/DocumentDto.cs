namespace DocumentStorage.Shared.DTOs.Document
{
    public class DocumentDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string FileName { get; set; } = null!;
        public long? FileSize { get; set; }
        public string ContentType { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public long SizeBytes { get; set; }
        public string? AccountName { get; set; }
        public string? Description { get; set; }
    }
}