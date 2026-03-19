using DocumentStorage.Shared.DTOs.Usage;

namespace DocumentStorage.BusinessLayer.Services.Interfaces
{
    public interface IUsageService
    {
        Task<UsageDto> GetCurrentUsageAsync(Guid accountId);
        Task<bool> CanUploadFileAsync(Guid accountId, long fileSizeBytes);
        Task UpdateUsageAsync(Guid accountId, long bytesAdded);
        Task RecalculateUsageAsync(Guid accountId);
    }
}