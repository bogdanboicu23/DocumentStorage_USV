using System;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.BO.Models.Plan;
using DocumentStorage.Server.DTOs.Plan;
using DocumentStorage.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DocumentStorage.BO.Controllers
{
    public class PlansController : Controller
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }

        public async Task<IActionResult> Index()
        {
            var plans = await _planService.GetAllPlansAsync();
            var viewModels = plans.Select(p => new Models.Plan.PlanDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Currency = p.Currency,
                BillingCycle = p.BillingCycle,
                IsActive = p.IsActive,
                SortOrder = p.SortOrder
            });
            return View(viewModels);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var plan = await _planService.GetPlanByIdAsync(id.Value);
            if (plan == null)
                return NotFound();

            var viewModel = new Models.Plan.PlanDto
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
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new Models.Plan.CreatePlanDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Plan.CreatePlanDto createPlanDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var serviceDto = new Server.DTOs.Plan.CreatePlanDto
                    {
                        Code = createPlanDto.Code,
                        Name = createPlanDto.Name,
                        Description = createPlanDto.Description,
                        Price = createPlanDto.Price,
                        Currency = createPlanDto.Currency,
                        BillingCycle = createPlanDto.BillingCycle,
                        IsActive = createPlanDto.IsActive,
                        SortOrder = createPlanDto.SortOrder
                    };
                    await _planService.CreatePlanAsync(serviceDto);
                    TempData["SuccessMessage"] = "Plan created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(createPlanDto);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var plan = await _planService.GetPlanByIdAsync(id.Value);
            if (plan == null)
                return NotFound();

            var updatePlanDto = new Models.Plan.UpdatePlanDto
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

            return View(updatePlanDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Models.Plan.UpdatePlanDto updatePlanDto)
        {
            if (id != updatePlanDto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var serviceDto = new Server.DTOs.Plan.UpdatePlanDto
                    {
                        Id = updatePlanDto.Id,
                        Code = updatePlanDto.Code,
                        Name = updatePlanDto.Name,
                        Description = updatePlanDto.Description,
                        Price = updatePlanDto.Price,
                        Currency = updatePlanDto.Currency,
                        BillingCycle = updatePlanDto.BillingCycle,
                        IsActive = updatePlanDto.IsActive,
                        SortOrder = updatePlanDto.SortOrder
                    };
                    await _planService.UpdatePlanAsync(serviceDto);
                    TempData["SuccessMessage"] = "Plan updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(updatePlanDto);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var plan = await _planService.GetPlanByIdAsync(id.Value);
            if (plan == null)
                return NotFound();

            var viewModel = new Models.Plan.PlanDto
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
            ViewBag.CanDelete = await _planService.CanDeletePlanAsync(id.Value);
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _planService.DeletePlanAsync(id);
                TempData["SuccessMessage"] = "Plan deleted successfully!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}