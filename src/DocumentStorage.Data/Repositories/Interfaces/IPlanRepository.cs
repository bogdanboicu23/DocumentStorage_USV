using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentStorage.Data.Models;

namespace DocumentStorage.Data.Repositories.Interfaces
{
    public interface IPlanRepository
    {
        Task<IEnumerable<Plan>> GetAllPlansAsync();
        Task<Plan> GetPlanByIdAsync(Guid id);
        Task<Plan> GetPlanByCodeAsync(string code);
        Task<Plan> CreatePlanAsync(Plan plan);
        Task<Plan> UpdatePlanAsync(Plan plan);
        Task<bool> DeletePlanAsync(Guid id);
        Task<bool> PlanExistsAsync(Guid id);
        Task<bool> PlanCodeExistsAsync(string code, Guid? excludeId = null);
        Task<bool> HasActiveSubscriptionsAsync(Guid planId);
    }
}