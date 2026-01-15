using DocumentStorage.BusinessLayer.Services.Interfaces;
using DocumentStorage.Data;
using DocumentStorage.Data.Models;
using DocumentStorage.Data.Repositories.Interfaces;
using DocumentStorage.Shared.DTOs.Usage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DocumentStorage.BusinessLayer.Services
{
    public class UsageService : IUsageService
    {
        private readonly DocumentStorageDbContext _context;
        private readonly IDocumentRepository _documentRepository;
        private readonly ILogger<UsageService> _logger;

        public UsageService(
            DocumentStorageDbContext context,
            IDocumentRepository documentRepository,
            ILogger<UsageService> logger)
        {
            _context = context;
            _documentRepository = documentRepository;
            _logger = logger;
        }

        public async Task<UsageDto> GetCurrentUsageAsync(Guid accountId)
        {
            // Get current subscription
            var subscription = await _context.Subscriptions
                .Include(s => s.Plan)
                .ThenInclude(p => p.PlanLimits)
                .Where(s => s.AccountId == accountId && s.Status == "Active")
                .OrderByDescending(s => s.PeriodEnd)
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                // Return free tier limits if no subscription
                return new UsageDto
                {
                    UsedStorageBytes = 0,
                    MaxStorageBytes = 1073741824, // 1GB free tier
                    UsedDocuments = 0,
                    MaxDocuments = 100 // 100 documents free tier
                };
            }

            // Get storage limit from plan
            var storageLimitBytes = subscription.Plan.PlanLimits
                .FirstOrDefault(l => l.ResourceType == "storage")?.MaxValue ?? 1073741824; // Default 1GB

            var documentLimit = subscription.Plan.PlanLimits
                .FirstOrDefault(l => l.ResourceType == "documents")?.MaxValue ?? 0;

            // Calculate current usage
            var documents = await _context.Documents
                .Where(d => d.AccountId == accountId && !d.IsDeleted)
                .ToListAsync();

            var usedStorageBytes = documents.Sum(d => d.SizeBytes);
            var usedDocuments = documents.Count;

            return new UsageDto
            {
                UsedStorageBytes = usedStorageBytes,
                MaxStorageBytes = storageLimitBytes,
                UsedDocuments = usedDocuments,
                MaxDocuments = (int)documentLimit
            };
        }

        public async Task<bool> CanUploadFileAsync(Guid accountId, long fileSizeBytes)
        {
            var usage = await GetCurrentUsageAsync(accountId);

            // Check storage limit
            if (usage.UsedStorageBytes + fileSizeBytes > usage.MaxStorageBytes)
            {
                _logger.LogWarning($"Account {accountId} has reached storage limit. Used: {usage.UsedStorageBytes}, Max: {usage.MaxStorageBytes}, Trying to add: {fileSizeBytes}");
                return false;
            }

            // Check document count limit
            if (usage.MaxDocuments > 0 && usage.UsedDocuments >= usage.MaxDocuments)
            {
                _logger.LogWarning($"Account {accountId} has reached document limit. Used: {usage.UsedDocuments}, Max: {usage.MaxDocuments}");
                return false;
            }

            return true;
        }

        public async Task UpdateUsageAsync(Guid accountId, long bytesAdded)
        {
            // Get or create usage record for current period
            var subscription = await _context.Subscriptions
                .Where(s => s.AccountId == accountId && s.Status == "Active")
                .OrderByDescending(s => s.PeriodEnd)
                .FirstOrDefaultAsync();

            if (subscription != null)
            {
                var currentPeriod = DateTime.UtcNow;
                var existingRecord = await _context.UsageRecords
                    .Where(ur => ur.SubscriptionId == subscription.Id
                        && ur.ResourceType == "storage"
                        && ur.PeriodStart <= currentPeriod
                        && ur.PeriodEnd >= currentPeriod)
                    .FirstOrDefaultAsync();

                if (existingRecord != null)
                {
                    existingRecord.UsedValue += bytesAdded;
                }
                else
                {
                    _context.UsageRecords.Add(new UsageRecord
                    {
                        Id = Guid.NewGuid(),
                        SubscriptionId = subscription.Id,
                        ResourceType = "storage",
                        UsedValue = bytesAdded,
                        PeriodStart = subscription.PeriodStart,
                        PeriodEnd = subscription.PeriodEnd
                    });
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task RecalculateUsageAsync(Guid accountId)
        {
            // Recalculate total usage from documents
            var documents = await _context.Documents
                .Where(d => d.AccountId == accountId && !d.IsDeleted)
                .ToListAsync();

            var totalBytes = documents.Sum(d => d.SizeBytes);

            var subscription = await _context.Subscriptions
                .Where(s => s.AccountId == accountId && s.Status == "Active")
                .OrderByDescending(s => s.PeriodEnd)
                .FirstOrDefaultAsync();

            if (subscription != null)
            {
                var currentPeriod = DateTime.UtcNow;
                var existingRecord = await _context.UsageRecords
                    .Where(ur => ur.SubscriptionId == subscription.Id
                        && ur.ResourceType == "storage"
                        && ur.PeriodStart <= currentPeriod
                        && ur.PeriodEnd >= currentPeriod)
                    .FirstOrDefaultAsync();

                if (existingRecord != null)
                {
                    existingRecord.UsedValue = totalBytes;
                }
                else
                {
                    _context.UsageRecords.Add(new UsageRecord
                    {
                        Id = Guid.NewGuid(),
                        SubscriptionId = subscription.Id,
                        ResourceType = "storage",
                        UsedValue = totalBytes,
                        PeriodStart = subscription.PeriodStart,
                        PeriodEnd = subscription.PeriodEnd
                    });
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}