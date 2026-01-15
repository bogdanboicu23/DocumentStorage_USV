using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentStorage.MVC.Models.Plan
{
    public class PlanModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Plan Code")]
        public string Code { get; set; } = null!;

        [Display(Name = "Plan Name")]
        public string Name { get; set; } = null!;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Currency")]
        public string Currency { get; set; } = null!;

        [Display(Name = "Billing Cycle")]
        public string BillingCycle { get; set; } = null!;

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }
    }
}