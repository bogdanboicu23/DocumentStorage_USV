using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentStorage.Shared.DTOs.Document
{
    public class CreateDocumentDto
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = null!;

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "File size must be greater than 0")]
        public long SizeBytes { get; set; }

        [Required]
        [StringLength(100)]
        public string ContentType { get; set; } = null!;

        public byte[]? FileContent { get; set; }
    }
}