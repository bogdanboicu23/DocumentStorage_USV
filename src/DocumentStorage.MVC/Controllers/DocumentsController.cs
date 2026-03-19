using DocumentStorage.BusinessLayer.Services.Interfaces;
using DocumentStorage.MVC.Models.Document;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace DocumentStorage.MVC.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IAccountService _accountService;
        private readonly IPlanService _planService;
        private readonly IUsageService _usageService;

        public DocumentsController(
            IDocumentService documentService,
            IAccountService accountService,
            IPlanService planService,
            IUsageService usageService)
        {
            _documentService = documentService;
            _accountService = accountService;
            _planService = planService;
            _usageService = usageService;
        }

        public async Task<IActionResult> Index()
        {
            var documents = await _documentService.GetAllDocumentsAsync();
            var viewModels = documents.Select(d => new DocumentModel
            {
                Id = d.Id,
                AccountId = d.AccountId,
                FileName = d.FileName,
                SizeBytes = d.SizeBytes,
                ContentType = d.ContentType,
                CreatedAt = d.CreatedAt,
                IsDeleted = d.IsDeleted,
                AccountName = d.AccountName
            });
            return View(viewModels);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var document = await _documentService.GetDocumentByIdAsync(id.Value);
            if (document == null)
                return NotFound();

            var viewModel = new DocumentModel
            {
                Id = document.Id,
                AccountId = document.AccountId,
                FileName = document.FileName,
                SizeBytes = document.SizeBytes,
                ContentType = document.ContentType,
                CreatedAt = document.CreatedAt,
                IsDeleted = document.IsDeleted,
                AccountName = document.AccountName
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            ViewBag.AccountId = new SelectList(accounts, "Id", "Name");
            return View(new CreateDocumentModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDocumentModel createDocumentModel)
        {
            if (ModelState.IsValid && createDocumentModel.File != null)
            {
                try
                {
                    byte[] fileContent;
                    using (var memoryStream = new MemoryStream())
                    {
                        await createDocumentModel.File.CopyToAsync(memoryStream);
                        fileContent = memoryStream.ToArray();
                    }

                    var serviceDto = new DocumentStorage.Shared.DTOs.Document.CreateDocumentDto
                    {
                        AccountId = createDocumentModel.AccountId,
                        FileName = createDocumentModel.File.FileName,
                        SizeBytes = createDocumentModel.File.Length,
                        ContentType = createDocumentModel.File.ContentType ?? "application/octet-stream",
                        FileContent = fileContent
                    };

                    await _documentService.CreateDocumentAsync(serviceDto);
                    TempData["SuccessMessage"] = "Document uploaded successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    // Check if this is a limit exceeded error
                    if (ex.Message.Contains("limit exceeded", StringComparison.OrdinalIgnoreCase))
                    {
                        // Get usage info and available plans
                        var usage = await _usageService.GetCurrentUsageAsync(createDocumentModel.AccountId);
                        var plans = await _planService.GetAllPlansAsync();
                        var activePlans = plans.Where(p => p.IsActive).OrderBy(p => p.SortOrder);

                        TempData["ShowPaywall"] = true;
                        TempData["LimitMessage"] = ex.Message;
                        TempData["CurrentUsage"] = JsonSerializer.Serialize(usage);
                        TempData["AvailablePlans"] = JsonSerializer.Serialize(activePlans);
                    }
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var accounts = await _accountService.GetAllAccountsAsync();
            ViewBag.AccountId = new SelectList(accounts, "Id", "Name", createDocumentModel.AccountId);
            return View(createDocumentModel);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var document = await _documentService.GetDocumentByIdAsync(id.Value);
            if (document == null)
                return NotFound();

            var updateDocumentDto = new UpdateDocumentModel
            {
                Id = document.Id,
                FileName = document.FileName,
                IsDeleted = document.IsDeleted
            };

            return View(updateDocumentDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateDocumentModel updateDocumentModel)
        {
            if (id != updateDocumentModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var serviceDto = new DocumentStorage.Shared.DTOs.Document.UpdateDocumentDto
                    {
                        Id = updateDocumentModel.Id,
                        FileName = updateDocumentModel.FileName,
                        IsDeleted = updateDocumentModel.IsDeleted
                    };
                    await _documentService.UpdateDocumentAsync(serviceDto);
                    TempData["SuccessMessage"] = "Document updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(updateDocumentModel);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var document = await _documentService.GetDocumentByIdAsync(id.Value);
            if (document == null)
                return NotFound();

            var viewModel = new DocumentModel
            {
                Id = document.Id,
                AccountId = document.AccountId,
                FileName = document.FileName,
                SizeBytes = document.SizeBytes,
                ContentType = document.ContentType,
                CreatedAt = document.CreatedAt,
                IsDeleted = document.IsDeleted,
                AccountName = document.AccountName
            };
            ViewBag.CanDelete = await _documentService.CanDeleteDocumentAsync(id.Value);
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _documentService.DeleteDocumentAsync(id);
                TempData["SuccessMessage"] = "Document deleted successfully!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AccountDocuments(Guid? accountId)
        {
            if (accountId == null)
                return RedirectToAction(nameof(Index));

            var documents = await _documentService.GetDocumentsByAccountIdAsync(accountId.Value);
            var viewModels = documents.Select(d => new DocumentModel
            {
                Id = d.Id,
                AccountId = d.AccountId,
                FileName = d.FileName,
                SizeBytes = d.SizeBytes,
                ContentType = d.ContentType,
                CreatedAt = d.CreatedAt,
                IsDeleted = d.IsDeleted,
                AccountName = d.AccountName
            });

            var accountName = viewModels.FirstOrDefault()?.AccountName ?? "Unknown Account";
            ViewBag.AccountName = accountName;
            ViewBag.AccountId = accountId;

            var storageUsed = await _documentService.GetAccountStorageUsageAsync(accountId.Value);
            var documentCount = await _documentService.GetAccountDocumentCountAsync(accountId.Value);

            ViewBag.StorageUsed = FormatFileSize(storageUsed);
            ViewBag.DocumentCount = documentCount;

            return View(viewModels);
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}