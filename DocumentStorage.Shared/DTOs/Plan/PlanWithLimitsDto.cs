namespace DocumentStorage.Shared.DTOs.Plan
{
    public class PlanWithLimitsDto
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
        public long StorageLimitBytes { get; set; }
        public int? DocumentLimit { get; set; }
        public List<string> Features { get; set; } = new();

        public string FormattedStorage => FormatBytes(StorageLimitBytes);
        public string FormattedPrice => $"{Currency} {Price:0.00}/{BillingCycle}";
        public bool IsPopular { get; set; }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size = size / 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }
    }
}