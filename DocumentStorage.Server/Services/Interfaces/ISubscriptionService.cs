using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentStorage.Server.DTOs.Subscription;

namespace DocumentStorage.Server.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync();
        Task<SubscriptionDto> GetSubscriptionByIdAsync(Guid id);
        Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto createDto);
        Task<SubscriptionDto> UpdateSubscriptionAsync(UpdateSubscriptionDto updateDto);
        Task<bool> DeleteSubscriptionAsync(Guid id);
        Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByAccountAsync(Guid accountId);
        Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByPlanAsync(Guid planId);
    }
}