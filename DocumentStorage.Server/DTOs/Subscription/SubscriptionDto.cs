using System;

namespace DocumentStorage.Server.DTOs.Subscription
{
    public class SubscriptionDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid PlanId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        // Navigation properties for display
        public string? AccountType { get; set; }
        public string? PlanName { get; set; }
        public string? PlanCode { get; set; }
        public decimal? PlanPrice { get; set; }
        public string? PlanCurrency { get; set; }
    }
}