using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentStorage.BO.Models.Plan
{
    public class UpdatePlanDto
    {
        [Required]
        public Guid Id { get; set; }

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
        public string Currency { get; set; } = null!;

        [Required(ErrorMessage = "Billing cycle is required")]
        [StringLength(50, ErrorMessage = "Billing cycle cannot exceed 50 characters")]
        [Display(Name = "Billing Cycle")]
        public string BillingCycle { get; set; } = null!;

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Sort order is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Sort order must be a positive number")]
        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }
    }
}