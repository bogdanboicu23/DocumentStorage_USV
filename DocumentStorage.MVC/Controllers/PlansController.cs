using System;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.MVC.Models.Plan;
using DocumentStorage.BusinessLayer.Services.Interfaces;
using DocumentStorage.Shared.DTOs.Plan;
using Microsoft.AspNetCore.Mvc;
using DocumentStorage.Data;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.MVC.Controllers
{
    public class PlansController : Controller
    {
        private readonly IPlanService _planService;
        private readonly DocumentStorageDbContext _context;
        private readonly ILogger<PlansController> _logger;

        public PlansController(IPlanService planService, DocumentStorageDbContext context, ILogger<PlansController> logger)
        {
            _planService = planService;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var plans = await _planService.GetAllPlansAsync();
            var viewModels = plans.Select(p => new PlanModel
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

            var viewModel = new PlanModel
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
            return View(new CreatePlanModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePlanModel createPlanModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var serviceDto = new CreatePlanDto
                    {
                        Code = createPlanModel.Code,
                        Name = createPlanModel.Name,
                        Description = createPlanModel.Description,
                        Price = createPlanModel.Price,
                        Currency = createPlanModel.Currency,
                        BillingCycle = createPlanModel.BillingCycle,
                        IsActive = createPlanModel.IsActive,
                        SortOrder = createPlanModel.SortOrder,
                        StorageLimitGB = createPlanModel.StorageLimitGB,
                        DocumentLimit = createPlanModel.DocumentLimit,
                        MaxFileSizeMB = createPlanModel.MaxFileSizeMB
                    };
                    await _planService.CreatePlanAsync(serviceDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(createPlanModel);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var plan = await _planService.GetPlanByIdAsync(id.Value);
            if (plan == null)
                return NotFound();

            // Load plan limits
            var planLimits = await _context.PlanLimits
                .Where(pl => pl.PlanId == plan.Id)
                .ToListAsync();

            var storageLimit = planLimits.FirstOrDefault(l => l.ResourceType == "storage");
            var documentLimit = planLimits.FirstOrDefault(l => l.ResourceType == "documents");
            var maxFileSizeLimit = planLimits.FirstOrDefault(l => l.ResourceType == "max_file_size");

            var updatePlanDto = new UpdatePlanModel
            {
                Id = plan.Id,
                Code = plan.Code,
                Name = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                Currency = plan.Currency,
                BillingCycle = plan.BillingCycle,
                IsActive = plan.IsActive,
                SortOrder = plan.SortOrder,
                StorageLimitGB = storageLimit != null ? storageLimit.MaxValue / 1073741824.0 : 1.0,
                DocumentLimit = documentLimit != null ? (int)documentLimit.MaxValue : 100,
                MaxFileSizeMB = maxFileSizeLimit != null ? (int)(maxFileSizeLimit.MaxValue / 1048576) : 100
            };

            return View(updatePlanDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdatePlanModel updatePlanModel)
        {
            if (id != updatePlanModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var serviceDto = new UpdatePlanDto
                    {
                        Id = updatePlanModel.Id,
                        Code = updatePlanModel.Code,
                        Name = updatePlanModel.Name,
                        Description = updatePlanModel.Description,
                        Price = updatePlanModel.Price,
                        Currency = updatePlanModel.Currency,
                        BillingCycle = updatePlanModel.BillingCycle,
                        IsActive = updatePlanModel.IsActive,
                        SortOrder = updatePlanModel.SortOrder,
                        StorageLimitGB = updatePlanModel.StorageLimitGB,
                        DocumentLimit = updatePlanModel.DocumentLimit,
                        MaxFileSizeMB = updatePlanModel.MaxFileSizeMB
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
            return View(updatePlanModel);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var plan = await _planService.GetPlanByIdAsync(id.Value);
            if (plan == null)
                return NotFound();

            var viewModel = new PlanModel
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