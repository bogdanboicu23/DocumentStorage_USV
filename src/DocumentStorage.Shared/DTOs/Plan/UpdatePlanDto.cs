using System;

namespace DocumentStorage.Shared.DTOs.Plan
{
    public class UpdatePlanDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = null!;
        public string BillingCycle { get; set; } = null!;
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }

        // Plan Limits
        public double StorageLimitGB { get; set; } = 1.0;
        public int DocumentLimit { get; set; } = 100;
        public int MaxFileSizeMB { get; set; } = 100;
    }
}