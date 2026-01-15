using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentStorage.MVC.Models.Document
{
    public class UpdateDocumentModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "File Name")]
        public string FileName { get; set; } = null!;

        [Display(Name = "Mark as Deleted")]
        public bool IsDeleted { get; set; }
    }
}