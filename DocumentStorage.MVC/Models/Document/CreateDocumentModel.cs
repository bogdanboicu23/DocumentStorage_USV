using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DocumentStorage.MVC.Models.Document
{
    public class CreateDocumentModel
    {
        [Required]
        [Display(Name = "Account")]
        public Guid AccountId { get; set; }

        [Required]
        [Display(Name = "File")]
        public IFormFile File { get; set; } = null!;
    }
}