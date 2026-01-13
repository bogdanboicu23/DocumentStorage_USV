using System;

namespace DocumentStorage.Server.DTOs.Subscription
{
    public class CreateSubscriptionDto
    {
        public Guid AccountId { get; set; }
        public Guid PlanId { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime PeriodStart { get; set; } = DateTime.Now;
        public DateTime PeriodEnd { get; set; } = DateTime.Now.AddMonths(1);
    }
}