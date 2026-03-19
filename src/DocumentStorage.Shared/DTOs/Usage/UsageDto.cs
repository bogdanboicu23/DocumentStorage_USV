namespace DocumentStorage.Shared.DTOs.Usage
{
    public class UsageDto
    {
        public long UsedStorageBytes { get; set; }
        public long MaxStorageBytes { get; set; }
        public int UsedDocuments { get; set; }
        public int MaxDocuments { get; set; }
        public double StoragePercentage => MaxStorageBytes > 0 ? (double)UsedStorageBytes / MaxStorageBytes * 100 : 0;
        public double DocumentPercentage => MaxDocuments > 0 ? (double)UsedDocuments / MaxDocuments * 100 : 0;
        public string FormattedUsedStorage => FormatBytes(UsedStorageBytes);
        public string FormattedMaxStorage => FormatBytes(MaxStorageBytes);
        public bool IsStorageLimitReached => UsedStorageBytes >= MaxStorageBytes;
        public bool IsDocumentLimitReached => MaxDocuments > 0 && UsedDocuments >= MaxDocuments;

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