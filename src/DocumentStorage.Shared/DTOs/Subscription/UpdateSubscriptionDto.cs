using System;

namespace DocumentStorage.Shared.DTOs.Subscription
{
    public class UpdateSubscriptionDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid PlanId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}