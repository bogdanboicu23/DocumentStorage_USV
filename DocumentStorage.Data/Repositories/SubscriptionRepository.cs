using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.Data.Models;
using DocumentStorage.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.Data.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly DocumentStorageDbContext _context;

        public SubscriptionRepository(DocumentStorageDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync()
        {
            return await _context.Subscriptions
                .OrderByDescending(s => s.PeriodStart)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsWithDetailsAsync()
        {
            return await _context.Subscriptions
                .Include(s => s.Account)
                .Include(s => s.Plan)
                .OrderByDescending(s => s.PeriodStart)
                .ToListAsync();
        }

        public async Task<Subscription> GetSubscriptionByIdAsync(Guid id)
        {
            return await _context.Subscriptions.FindAsync(id);
        }

        public async Task<Subscription> GetSubscriptionWithDetailsAsync(Guid id)
        {
            return await _context.Subscriptions
                .Include(s => s.Account)
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<Subscription> UpdateSubscriptionAsync(Subscription subscription)
        {
            _context.Entry(subscription).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<bool> DeleteSubscriptionAsync(Guid id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return false;
            }

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SubscriptionExistsAsync(Guid id)
        {
            return await _context.Subscriptions.AnyAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsByAccountAsync(Guid accountId)
        {
            return await _context.Subscriptions
                .Include(s => s.Plan)
                .Where(s => s.AccountId == accountId)
                .OrderByDescending(s => s.PeriodStart)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsByPlanAsync(Guid planId)
        {
            return await _context.Subscriptions
                .Include(s => s.Account)
                .Where(s => s.PlanId == planId)
                .OrderByDescending(s => s.PeriodStart)
                .ToListAsync();
        }

        public async Task<bool> HasActiveSubscriptionAsync(Guid accountId, Guid planId)
        {
            return await _context.Subscriptions
                .AnyAsync(s => s.AccountId == accountId &&
                              s.PlanId == planId &&
                              s.Status == "Active" &&
                              s.PeriodEnd > DateTime.Now);
        }
    }
}