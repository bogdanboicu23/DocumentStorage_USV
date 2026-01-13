using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DocumentStorage.BO.Models.Subscription
{
    public class CreateSubscriptionDto
    {
        [Required(ErrorMessage = "Account is required")]
        [Display(Name = "Account")]
        public Guid AccountId { get; set; }

        [Required(ErrorMessage = "Plan is required")]
        [Display(Name = "Plan")]
        public Guid PlanId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";

        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        public DateTime PeriodStart { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.DateTime)]
        public DateTime PeriodEnd { get; set; } = DateTime.Now.AddMonths(1);

        // For dropdowns
        public IEnumerable<SelectListItem>? Accounts { get; set; }
        public IEnumerable<SelectListItem>? Plans { get; set; }
        public IEnumerable<SelectListItem>? Statuses { get; set; }
    }
}