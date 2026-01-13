namespace DocumentStorage.Server.DTOs.Plan
{
    public class CreatePlanDto
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
        public string BillingCycle { get; set; } = "Monthly";
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }
    }
}