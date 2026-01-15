using System;

namespace DocumentStorage.Client.Models
{
    public class DocumentDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public long SizeBytes { get; set; }
        public string? FileUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? AccountName { get; set; }

        // Computed properties for backwards compatibility
        public string? FileType => ContentType;
        public long? FileSize => SizeBytes;
        public string? DisplaySize => SizeBytes > 0 ? FormatFileSize(SizeBytes) : "Unknown";

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }
    }
}