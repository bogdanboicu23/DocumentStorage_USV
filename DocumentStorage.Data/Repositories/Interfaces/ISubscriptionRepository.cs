using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentStorage.Data.Models;

namespace DocumentStorage.Data.Repositories.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync();
        Task<IEnumerable<Subscription>> GetSubscriptionsWithDetailsAsync();
        Task<Subscription> GetSubscriptionByIdAsync(Guid id);
        Task<Subscription> GetSubscriptionWithDetailsAsync(Guid id);
        Task<Subscription> CreateSubscriptionAsync(Subscription subscription);
        Task<Subscription> UpdateSubscriptionAsync(Subscription subscription);
        Task<bool> DeleteSubscriptionAsync(Guid id);
        Task<bool> SubscriptionExistsAsync(Guid id);
        Task<IEnumerable<Subscription>> GetSubscriptionsByAccountAsync(Guid accountId);
        Task<IEnumerable<Subscription>> GetSubscriptionsByPlanAsync(Guid planId);
        Task<bool> HasActiveSubscriptionAsync(Guid accountId, Guid planId);
    }
}