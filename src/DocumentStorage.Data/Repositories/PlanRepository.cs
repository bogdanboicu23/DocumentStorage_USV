using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.Data.Models;
using DocumentStorage.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.Data.Repositories
{
    public class PlanRepository : IPlanRepository
    {
        private readonly DocumentStorageDbContext _context;

        public PlanRepository(DocumentStorageDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Plan>> GetAllPlansAsync()
        {
            return await _context.Plans
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Plan> GetPlanByIdAsync(Guid id)
        {
            return await _context.Plans.FindAsync(id);
        }

        public async Task<Plan> GetPlanByCodeAsync(string code)
        {
            return await _context.Plans
                .FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task<Plan> CreatePlanAsync(Plan plan)
        {
            _context.Plans.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<Plan> UpdatePlanAsync(Plan plan)
        {
            _context.Entry(plan).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<bool> DeletePlanAsync(Guid id)
        {
            var plan = await _context.Plans.FindAsync(id);
            if (plan == null)
            {
                return false;
            }

            _context.Plans.Remove(plan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PlanExistsAsync(Guid id)
        {
            return await _context.Plans.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> PlanCodeExistsAsync(string code, Guid? excludeId = null)
        {
            var query = _context.Plans.Where(p => p.Code == code);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasActiveSubscriptionsAsync(Guid planId)
        {
            return await _context.Subscriptions
                .AnyAsync(s => s.PlanId == planId);
        }
    }
}