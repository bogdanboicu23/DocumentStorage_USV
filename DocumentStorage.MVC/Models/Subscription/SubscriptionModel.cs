using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentStorage.MVC.Models.Subscription
{
    public class SubscriptionModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Account")]
        public Guid AccountId { get; set; }

        [Display(Name = "Plan")]
        public Guid PlanId { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = null!;

        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        public DateTime PeriodStart { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.DateTime)]
        public DateTime PeriodEnd { get; set; }

        // Display properties
        [Display(Name = "Account Type")]
        public string? AccountType { get; set; }

        [Display(Name = "Plan")]
        public string? PlanName { get; set; }

        [Display(Name = "Plan Code")]
        public string? PlanCode { get; set; }

        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal? PlanPrice { get; set; }

        [Display(Name = "Currency")]
        public string? PlanCurrency { get; set; }
    }
}