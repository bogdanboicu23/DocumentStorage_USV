using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentStorage.MVC.Models.Document
{
    public class DocumentModel
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; } = null!;

        [Display(Name = "Size (bytes)")]
        public long SizeBytes { get; set; }

        [Display(Name = "Content Type")]
        public string ContentType { get; set; } = null!;

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        [Display(Name = "Account")]
        public string? AccountName { get; set; }

        [Display(Name = "Size")]
        public string FormattedSize
        {
            get
            {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                double len = SizeBytes;
                int order = 0;
                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len = len / 1024;
                }
                return $"{len:0.##} {sizes[order]}";
            }
        }
    }
}