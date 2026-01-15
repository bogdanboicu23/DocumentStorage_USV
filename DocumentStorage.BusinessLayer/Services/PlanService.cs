using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.BusinessLayer.Services.Interfaces;
using DocumentStorage.Shared.DTOs.Plan;
using DocumentStorage.Data.Models;
using DocumentStorage.Data.Repositories.Interfaces;
using DocumentStorage.Data;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.BusinessLayer.Services
{
    public class PlanService : IPlanService
    {
        private readonly IPlanRepository _planRepository;
        private readonly DocumentStorageDbContext _context;

        public PlanService(IPlanRepository planRepository, DocumentStorageDbContext context)
        {
            _planRepository = planRepository;
            _context = context;
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

            // Create plan limits
            var planLimits = new List<PlanLimit>
            {
                new PlanLimit
                {
                    Id = Guid.NewGuid(),
                    PlanId = createdPlan.Id,
                    ResourceType = "storage",
                    MaxValue = (long)(createPlanDto.StorageLimitGB * 1073741824), // Convert GB to bytes
                    IsHardLimit = true
                },
                new PlanLimit
                {
                    Id = Guid.NewGuid(),
                    PlanId = createdPlan.Id,
                    ResourceType = "documents",
                    MaxValue = createPlanDto.DocumentLimit,
                    IsHardLimit = true
                },
                new PlanLimit
                {
                    Id = Guid.NewGuid(),
                    PlanId = createdPlan.Id,
                    ResourceType = "max_file_size",
                    MaxValue = (long)(createPlanDto.MaxFileSizeMB * 1048576), // Convert MB to bytes
                    IsHardLimit = true
                }
            };

            await _context.PlanLimits.AddRangeAsync(planLimits);
            await _context.SaveChangesAsync();

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

            // Update plan limits
            var existingLimits = await _context.PlanLimits
                .Where(pl => pl.PlanId == updatedPlan.Id)
                .ToListAsync();

            // Update storage limit
            var storageLimit = existingLimits.FirstOrDefault(l => l.ResourceType == "storage");
            if (storageLimit != null)
            {
                storageLimit.MaxValue = (long)(updatePlanDto.StorageLimitGB * 1073741824);
            }
            else
            {
                _context.PlanLimits.Add(new PlanLimit
                {
                    Id = Guid.NewGuid(),
                    PlanId = updatedPlan.Id,
                    ResourceType = "storage",
                    MaxValue = (long)(updatePlanDto.StorageLimitGB * 1073741824),
                    IsHardLimit = true
                });
            }

            // Update document limit
            var documentLimit = existingLimits.FirstOrDefault(l => l.ResourceType == "documents");
            if (documentLimit != null)
            {
                documentLimit.MaxValue = updatePlanDto.DocumentLimit;
            }
            else
            {
                _context.PlanLimits.Add(new PlanLimit
                {
                    Id = Guid.NewGuid(),
                    PlanId = updatedPlan.Id,
                    ResourceType = "documents",
                    MaxValue = updatePlanDto.DocumentLimit,
                    IsHardLimit = true
                });
            }

            // Update max file size limit
            var maxFileSizeLimit = existingLimits.FirstOrDefault(l => l.ResourceType == "max_file_size");
            if (maxFileSizeLimit != null)
            {
                maxFileSizeLimit.MaxValue = (long)(updatePlanDto.MaxFileSizeMB * 1048576);
            }
            else
            {
                _context.PlanLimits.Add(new PlanLimit
                {
                    Id = Guid.NewGuid(),
                    PlanId = updatedPlan.Id,
                    ResourceType = "max_file_size",
                    MaxValue = (long)(updatePlanDto.MaxFileSizeMB * 1048576),
                    IsHardLimit = true
                });
            }

            await _context.SaveChangesAsync();

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