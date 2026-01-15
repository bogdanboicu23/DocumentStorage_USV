using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.BusinessLayer.Services.Interfaces;
using DocumentStorage.Data.Models;
using DocumentStorage.Data.Repositories.Interfaces;
using DocumentStorage.Shared.DTOs.Subscription;

namespace DocumentStorage.BusinessLayer.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync()
        {
            var subscriptions = await _subscriptionRepository.GetSubscriptionsWithDetailsAsync();
            return subscriptions.Select(MapToDto);
        }

        public async Task<SubscriptionDto> GetSubscriptionByIdAsync(Guid id)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionWithDetailsAsync(id);
            return subscription != null ? MapToDto(subscription) : null;
        }

        public async Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto createDto)
        {
            // Check for existing active subscription
            if (await _subscriptionRepository.HasActiveSubscriptionAsync(createDto.AccountId, createDto.PlanId))
            {
                throw new InvalidOperationException("This account already has an active subscription for this plan.");
            }

            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                AccountId = createDto.AccountId,
                PlanId = createDto.PlanId,
                Status = createDto.Status,
                PeriodStart = createDto.PeriodStart,
                PeriodEnd = createDto.PeriodEnd
            };

            var created = await _subscriptionRepository.CreateSubscriptionAsync(subscription);
            var result = await _subscriptionRepository.GetSubscriptionWithDetailsAsync(created.Id);
            return MapToDto(result);
        }

        public async Task<SubscriptionDto> UpdateSubscriptionAsync(UpdateSubscriptionDto updateDto)
        {
            var existing = await _subscriptionRepository.GetSubscriptionByIdAsync(updateDto.Id);
            if (existing == null)
            {
                throw new InvalidOperationException($"Subscription with ID '{updateDto.Id}' not found.");
            }

            // Check if changing to a different plan that already has active subscription
            if (existing.AccountId != updateDto.AccountId || existing.PlanId != updateDto.PlanId)
            {
                if (updateDto.Status == "Active" &&
                    await _subscriptionRepository.HasActiveSubscriptionAsync(updateDto.AccountId, updateDto.PlanId))
                {
                    throw new InvalidOperationException("This account already has an active subscription for this plan.");
                }
            }

            existing.AccountId = updateDto.AccountId;
            existing.PlanId = updateDto.PlanId;
            existing.Status = updateDto.Status;
            existing.PeriodStart = updateDto.PeriodStart;
            existing.PeriodEnd = updateDto.PeriodEnd;

            await _subscriptionRepository.UpdateSubscriptionAsync(existing);
            var result = await _subscriptionRepository.GetSubscriptionWithDetailsAsync(existing.Id);
            return MapToDto(result);
        }

        public async Task<bool> DeleteSubscriptionAsync(Guid id)
        {
            return await _subscriptionRepository.DeleteSubscriptionAsync(id);
        }

        public async Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByAccountAsync(Guid accountId)
        {
            var subscriptions = await _subscriptionRepository.GetSubscriptionsByAccountAsync(accountId);
            return subscriptions.Select(MapToDto);
        }

        public async Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByPlanAsync(Guid planId)
        {
            var subscriptions = await _subscriptionRepository.GetSubscriptionsByPlanAsync(planId);
            return subscriptions.Select(MapToDto);
        }

        private SubscriptionDto MapToDto(Subscription subscription)
        {
            return new SubscriptionDto
            {
                Id = subscription.Id,
                AccountId = subscription.AccountId,
                PlanId = subscription.PlanId,
                Status = subscription.Status,
                PeriodStart = subscription.PeriodStart,
                PeriodEnd = subscription.PeriodEnd,
                AccountType = subscription.Account?.AccountType,
                PlanName = subscription.Plan?.Name,
                PlanCode = subscription.Plan?.Code,
                PlanPrice = subscription.Plan?.Price,
                PlanCurrency = subscription.Plan?.Currency
            };
        }
    }
}