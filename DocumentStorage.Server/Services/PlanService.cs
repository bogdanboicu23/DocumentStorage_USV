using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.Server.DTOs.Plan;
using DocumentStorage.Data.Models;
using DocumentStorage.Data.Repositories.Interfaces;
using DocumentStorage.Server.Services.Interfaces;

namespace DocumentStorage.Server.Services
{
    public class PlanService : IPlanService
    {
        private readonly IPlanRepository _planRepository;

        public PlanService(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        public async Task<IEnumerable<PlanDto>> GetAllPlansAsync()
        {
            var plans = await _planRepository.GetAllPlansAsync();
            return plans.Select(MapToDto);
        }

        public async Task<PlanDto> GetPlanByIdAsync(Guid id)
        {
            var plan = await _planRepository.GetPlanByIdAsync(id);
            return plan != null ? MapToDto(plan) : null;
        }

        public async Task<PlanDto> CreatePlanAsync(CreatePlanDto createPlanDto)
        {
            if (await _planRepository.PlanCodeExistsAsync(createPlanDto.Code))
            {
                throw new InvalidOperationException($"Plan with code '{createPlanDto.Code}' already exists.");
            }

            var plan = new Plan
            {
                Id = Guid.NewGuid(),
                Code = createPlanDto.Code,
                Name = createPlanDto.Name,
                Description = createPlanDto.Description,
                Price = createPlanDto.Price,
                Currency = createPlanDto.Currency,
                BillingCycle = createPlanDto.BillingCycle,
                IsActive = createPlanDto.IsActive,
                SortOrder = createPlanDto.SortOrder
            };

            var createdPlan = await _planRepository.CreatePlanAsync(plan);
            return MapToDto(createdPlan);
        }

        public async Task<PlanDto> UpdatePlanAsync(UpdatePlanDto updatePlanDto)
        {
            var existingPlan = await _planRepository.GetPlanByIdAsync(updatePlanDto.Id);
            if (existingPlan == null)
            {
                throw new InvalidOperationException($"Plan with ID '{updatePlanDto.Id}' not found.");
            }

            if (await _planRepository.PlanCodeExistsAsync(updatePlanDto.Code, updatePlanDto.Id))
            {
                throw new InvalidOperationException($"Plan with code '{updatePlanDto.Code}' already exists.");
            }

            existingPlan.Code = updatePlanDto.Code;
            existingPlan.Name = updatePlanDto.Name;
            existingPlan.Description = updatePlanDto.Description;
            existingPlan.Price = updatePlanDto.Price;
            existingPlan.Currency = updatePlanDto.Currency;
            existingPlan.BillingCycle = updatePlanDto.BillingCycle;
            existingPlan.IsActive = updatePlanDto.IsActive;
            existingPlan.SortOrder = updatePlanDto.SortOrder;

            var updatedPlan = await _planRepository.UpdatePlanAsync(existingPlan);
            return MapToDto(updatedPlan);
        }

        public async Task<bool> DeletePlanAsync(Guid id)
        {
            if (await _planRepository.HasActiveSubscriptionsAsync(id))
            {
                throw new InvalidOperationException("Cannot delete plan with active subscriptions.");
            }

            return await _planRepository.DeletePlanAsync(id);
        }

        public async Task<bool> CanDeletePlanAsync(Guid id)
        {
            return !await _planRepository.HasActiveSubscriptionsAsync(id);
        }

        private PlanDto MapToDto(Plan plan)
        {
            return new PlanDto
            {
                Id = plan.Id,
                Code = plan.Code,
                Name = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                Currency = plan.Currency,
                BillingCycle = plan.BillingCycle,
                IsActive = plan.IsActive,
                SortOrder = plan.SortOrder
            };
        }
    }
}