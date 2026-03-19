using System;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.MVC.Models.Subscription;
using DocumentStorage.BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DocumentStorage.MVC.Controllers
{
    public class SubscriptionsController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IAccountService _accountService;
        private readonly IPlanService _planService;

        public SubscriptionsController(
            ISubscriptionService subscriptionService,
            IAccountService accountService,
            IPlanService planService)
        {
            _subscriptionService = subscriptionService;
            _accountService = accountService;
            _planService = planService;
        }

        public async Task<IActionResult> Index()
        {
            var subscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
            var viewModels = subscriptions.Select(s => new SubscriptionModel
            {
                Id = s.Id,
                AccountId = s.AccountId,
                PlanId = s.PlanId,
                Status = s.Status,
                PeriodStart = s.PeriodStart,
                PeriodEnd = s.PeriodEnd,
                AccountType = s.AccountType,
                PlanName = s.PlanName,
                PlanCode = s.PlanCode,
                PlanPrice = s.PlanPrice,
                PlanCurrency = s.PlanCurrency
            });
            return View(viewModels);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id.Value);
            if (subscription == null)
                return NotFound();

            var viewModel = new SubscriptionModel
            {
                Id = subscription.Id,
                AccountId = subscription.AccountId,
                PlanId = subscription.PlanId,
                Status = subscription.Status,
                PeriodStart = subscription.PeriodStart,
                PeriodEnd = subscription.PeriodEnd,
                AccountType = subscription.AccountType,
                PlanName = subscription.PlanName,
                PlanCode = subscription.PlanCode,
                PlanPrice = subscription.PlanPrice,
                PlanCurrency = subscription.PlanCurrency
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var model = new CreateSubscriptionModel();
            await PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSubscriptionModel createModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var serviceDto = new Shared.DTOs.Subscription.CreateSubscriptionDto
                    {
                        AccountId = createModel.AccountId,
                        PlanId = createModel.PlanId,
                        Status = createModel.Status,
                        PeriodStart = createModel.PeriodStart,
                        PeriodEnd = createModel.PeriodEnd
                    };

                    await _subscriptionService.CreateSubscriptionAsync(serviceDto);
                    TempData["SuccessMessage"] = "Subscription created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            await PopulateDropdowns(createModel);
            return View(createModel);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id.Value);
            if (subscription == null)
                return NotFound();

            var updateDto = new UpdateSubscriptionModel
            {
                Id = subscription.Id,
                AccountId = subscription.AccountId,
                PlanId = subscription.PlanId,
                Status = subscription.Status,
                PeriodStart = subscription.PeriodStart,
                PeriodEnd = subscription.PeriodEnd
            };

            await PopulateDropdowns(updateDto);
            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateSubscriptionModel updateModel)
        {
            if (id != updateModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var serviceDto = new Shared.DTOs.Subscription.UpdateSubscriptionDto
                    {
                        Id = updateModel.Id,
                        AccountId = updateModel.AccountId,
                        PlanId = updateModel.PlanId,
                        Status = updateModel.Status,
                        PeriodStart = updateModel.PeriodStart,
                        PeriodEnd = updateModel.PeriodEnd
                    };

                    await _subscriptionService.UpdateSubscriptionAsync(serviceDto);
                    TempData["SuccessMessage"] = "Subscription updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            await PopulateDropdowns(updateModel);
            return View(updateModel);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id.Value);
            if (subscription == null)
                return NotFound();

            var viewModel = new SubscriptionModel
            {
                Id = subscription.Id,
                AccountId = subscription.AccountId,
                PlanId = subscription.PlanId,
                Status = subscription.Status,
                PeriodStart = subscription.PeriodStart,
                PeriodEnd = subscription.PeriodEnd,
                AccountType = subscription.AccountType,
                PlanName = subscription.PlanName,
                PlanCode = subscription.PlanCode,
                PlanPrice = subscription.PlanPrice,
                PlanCurrency = subscription.PlanCurrency
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _subscriptionService.DeleteSubscriptionAsync(id);
                TempData["SuccessMessage"] = "Subscription deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting subscription: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns(CreateSubscriptionModel model)
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            var plans = await _planService.GetAllPlansAsync();

            model.Accounts = accounts.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.AccountType} (ID: {a.Id})"
            });

            model.Plans = plans.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} - {p.Price} {p.Currency}/{p.BillingCycle}"
            });

            model.Statuses = new[]
            {
                new SelectListItem { Value = "Active", Text = "Active" },
                new SelectListItem { Value = "Inactive", Text = "Inactive" },
                new SelectListItem { Value = "Cancelled", Text = "Cancelled" },
                new SelectListItem { Value = "Expired", Text = "Expired" }
            };
        }

        private async Task PopulateDropdowns(UpdateSubscriptionModel model)
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            var plans = await _planService.GetAllPlansAsync();

            model.Accounts = accounts.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.AccountType} (ID: {a.Id})"
            });

            model.Plans = plans.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} - {p.Price} {p.Currency}/{p.BillingCycle}"
            });

            model.Statuses = new[]
            {
                new SelectListItem { Value = "Active", Text = "Active" },
                new SelectListItem { Value = "Inactive", Text = "Inactive" },
                new SelectListItem { Value = "Cancelled", Text = "Cancelled" },
                new SelectListItem { Value = "Expired", Text = "Expired" }
            };
        }
    }
}