using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DocumentStorage.BusinessLayer.Services.Interfaces;
using DocumentStorage.Shared.DTOs.Plan;
using DocumentStorage.Shared.DTOs.Usage;
using DocumentStorage.Data;
using DocumentStorage.Data.Models;
using DocumentStorage.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DocumentStorage.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlansController : ControllerBase
    {
        private readonly DocumentStorageDbContext _context;
        private readonly IUsageService _usageService;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<PlansController> _logger;

        public PlansController(
            DocumentStorageDbContext context,
            IUsageService usageService,
            IAccountRepository accountRepository,
            ILogger<PlansController> logger)
        {
            _context = context;
            _usageService = usageService;
            _accountRepository = accountRepository;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PlanWithLimitsDto>>> GetPlans()
        {
            var plans = await _context.Plans
                .Include(p => p.PlanLimits)
                .Where(p => p.IsActive)
                .OrderBy(p => p.SortOrder)
                .ToListAsync();

            var planDtos = plans.Select(p => new PlanWithLimitsDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Currency = p.Currency,
                BillingCycle = p.BillingCycle,
                IsActive = p.IsActive,
                SortOrder = p.SortOrder,
                StorageLimitBytes = p.PlanLimits.FirstOrDefault(l => l.ResourceType == "storage")?.MaxValue ?? 0,
                DocumentLimit = (int?)(p.PlanLimits.FirstOrDefault(l => l.ResourceType == "documents")?.MaxValue),
                IsPopular = p.Code == "PROFESSIONAL", // Mark Professional as popular
                Features = GetPlanFeatures(p.Code)
            }).ToList();

            return Ok(planDtos);
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<ActionResult<PlanWithLimitsDto>> GetCurrentPlan()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var account = await _accountRepository.GetAccountByUserIdAsync(userId);
            if (account == null)
            {
                return NotFound("Account not found");
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Plan)
                .ThenInclude(p => p.PlanLimits)
                .Where(s => s.AccountId == account.Id && s.Status == "Active")
                .OrderByDescending(s => s.PeriodEnd)
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                // Return free tier if no subscription
                return Ok(new PlanWithLimitsDto
                {
                    Id = Guid.Empty,
                    Code = "FREE",
                    Name = "Free Tier",
                    Description = "Basic storage for personal use",
                    Price = 0,
                    Currency = "$",
                    BillingCycle = "forever",
                    IsActive = true,
                    SortOrder = 0,
                    StorageLimitBytes = 1073741824, // 1GB
                    DocumentLimit = 100,
                    Features = new List<string> { "1 GB Storage", "Up to 100 documents", "Basic support" }
                });
            }

            var plan = subscription.Plan;
            return Ok(new PlanWithLimitsDto
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
                StorageLimitBytes = plan.PlanLimits.FirstOrDefault(l => l.ResourceType == "storage")?.MaxValue ?? 0,
                DocumentLimit = (int?)(plan.PlanLimits.FirstOrDefault(l => l.ResourceType == "documents")?.MaxValue),
                Features = GetPlanFeatures(plan.Code)
            });
        }

        [HttpGet("usage")]
        [Authorize]
        public async Task<ActionResult<UsageDto>> GetUsage()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var account = await _accountRepository.GetAccountByUserIdAsync(userId);
            if (account == null)
            {
                return NotFound("Account not found");
            }

            var usage = await _usageService.GetCurrentUsageAsync(account.Id);
            return Ok(usage);
        }

        [HttpPost("upgrade/{planId}")]
        [Authorize]
        public async Task<IActionResult> UpgradePlan(Guid planId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var account = await _accountRepository.GetAccountByUserIdAsync(userId);
            if (account == null)
            {
                return NotFound("Account not found");
            }

            var plan = await _context.Plans.FindAsync(planId);
            if (plan == null || !plan.IsActive)
            {
                return BadRequest("Invalid plan selected");
            }

            // Cancel existing subscription
            var existingSubscription = await _context.Subscriptions
                .Where(s => s.AccountId == account.Id && s.Status == "Active")
                .FirstOrDefaultAsync();

            if (existingSubscription != null)
            {
                existingSubscription.Status = "Cancelled";
                existingSubscription.PeriodEnd = DateTime.UtcNow;
            }

            // Create new subscription
            var newSubscription = new Subscription
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                PlanId = planId,
                Status = "Active",
                PeriodStart = DateTime.UtcNow,
                PeriodEnd = plan.BillingCycle.ToLower() switch
                {
                    "monthly" => DateTime.UtcNow.AddMonths(1),
                    "yearly" => DateTime.UtcNow.AddYears(1),
                    _ => DateTime.UtcNow.AddMonths(1)
                }
            };

            _context.Subscriptions.Add(newSubscription);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User {userId} upgraded to plan {plan.Name} (ID: {planId})");

            return Ok(new { message = "Plan upgraded successfully", planName = plan.Name });
        }

        private List<string> GetPlanFeatures(string planCode)
        {
            return planCode.ToUpper() switch
            {
                "BASIC" => new List<string>
                {
                    "5 GB Storage",
                    "Up to 500 documents",
                    "Basic file management",
                    "Email support",
                    "Secure cloud storage"
                },
                "PROFESSIONAL" => new List<string>
                {
                    "50 GB Storage",
                    "Unlimited documents",
                    "Advanced file management",
                    "Priority support",
                    "File versioning",
                    "Team collaboration",
                    "Advanced search"
                },
                "ENTERPRISE" => new List<string>
                {
                    "500 GB Storage",
                    "Unlimited documents",
                    "All Professional features",
                    "24/7 phone support",
                    "Custom integrations",
                    "Advanced security",
                    "API access",
                    "Dedicated account manager"
                },
                _ => new List<string> { "1 GB Storage", "Up to 100 documents", "Basic support" }
            };
        }
    }
}