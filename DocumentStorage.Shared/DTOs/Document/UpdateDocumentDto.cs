using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentStorage.Shared.DTOs.Document
{
    public class UpdateDocumentDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}