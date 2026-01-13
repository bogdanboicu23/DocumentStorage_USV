using DocumentStorage.Server.DTOs.Plan;

namespace DocumentStorage.Server.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanDto>> GetAllPlansAsync();
        Task<PlanDto> GetPlanByIdAsync(Guid id);
        Task<PlanDto> CreatePlanAsync(CreatePlanDto createPlanDto);
        Task<PlanDto> UpdatePlanAsync(UpdatePlanDto updatePlanDto);
        Task<bool> DeletePlanAsync(Guid id);
        Task<bool> CanDeletePlanAsync(Guid id);
    }
}