using System.ComponentModel.DataAnnotations;

namespace DocumentStorage.MVC.Models.Plan
{
    public class CreatePlanModel
    {
        [Required(ErrorMessage = "Plan code is required")]
        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters")]
        [Display(Name = "Plan Code")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Plan name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Plan Name")]
        public string Name { get; set; } = null!;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        [StringLength(10, ErrorMessage = "Currency cannot exceed 10 characters")]
        [Display(Name = "Currency")]
        public string Currency { get; set; } = "USD";

        [Required(ErrorMessage = "Billing cycle is required")]
        [StringLength(50, ErrorMessage = "Billing cycle cannot exceed 50 characters")]
        [Display(Name = "Billing Cycle")]
        public string BillingCycle { get; set; } = "Monthly";

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Sort order is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Sort order must be a positive number")]
        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }

        // Plan Limits
        [Required(ErrorMessage = "Storage limit is required")]
        [Display(Name = "Storage Limit (GB)")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Storage must be at least 0.1 GB")]
        public double StorageLimitGB { get; set; } = 1.0;

        [Display(Name = "Document Limit")]
        [Range(0, int.MaxValue, ErrorMessage = "Document limit must be 0 (unlimited) or a positive number")]
        public int DocumentLimit { get; set; } = 100;

        [Display(Name = "Max File Size (MB)")]
        [Range(1, int.MaxValue, ErrorMessage = "Max file size must be at least 1 MB")]
        public int MaxFileSizeMB { get; set; } = 100;
    }
}