using System;

namespace DocumentStorage.Client.Models
{
    public class DocumentDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public long? FileSize { get; set; }
        public string? FileUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? DisplaySize => FileSize.HasValue ? FormatFileSize(FileSize.Value) : "Unknown";

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