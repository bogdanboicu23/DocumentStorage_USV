using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DocumentStorage.BusinessLayer.Services.Interfaces;
using DocumentStorage.Shared.DTOs.Document;
using DocumentStorage.Shared.DTOs.Usage;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentStorage.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IUsageService _usageService;
        private readonly IAuthService _authService;
        private readonly ILogger<DocumentsController> _logger;
        private readonly IMemoryCache _cache;

        public DocumentsController(
            IDocumentService documentService,
            IUsageService usageService,
            IAuthService authService,
            ILogger<DocumentsController> logger,
            IMemoryCache cache)
        {
            _documentService = documentService;
            _usageService = usageService;
            _authService = authService;
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments()
        {
            var documents = await _documentService.GetAllDocumentsAsync();
            return Ok(documents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentDto>> GetDocument(Guid id)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            return Ok(document);
        }

        [HttpPost]
        public async Task<ActionResult<DocumentDto>> CreateDocument([FromBody] CreateDocumentDto createDocumentDto)
        {
            try
            {
                var document = await _documentService.CreateDocumentAsync(createDocumentDto);
                return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, document);
            }
            catch (InvalidOperationException ex)
            {
                // Check if this is a limit exceeded error
                if (ex.Message.Contains("limit exceeded", StringComparison.OrdinalIgnoreCase))
                {
                    // Get current usage for the account
                    var usage = await _usageService.GetCurrentUsageAsync(createDocumentDto.AccountId);

                    return StatusCode(403, new
                    {
                        error = "LIMIT_EXCEEDED",
                        message = ex.Message,
                        currentUsage = usage,
                        upgradeRequired = true
                    });
                }

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<DocumentDto>> UploadDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Upload attempt with no file provided");
                return BadRequest(new { error = "No file provided" });
            }

            _logger.LogInformation($"Upload request received for file: {file.FileName}, Size: {file.Length} bytes");

            // Get the user's account ID from the JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Failed to extract user ID from JWT token");
                return Unauthorized("User ID not found in token");
            }

            _logger.LogInformation($"Processing upload for user {userId}");

            // Get account ID from the service
            var accountId = await _authService.GetUserAccountIdAsync(userId);
            if (accountId == null)
            {
                _logger.LogWarning($"Account not found for user {userId}");
                return NotFound("User account not found");
            }

            _logger.LogInformation($"Found account {accountId} for user {userId}");

            try
            {
                // Read file content
                byte[] fileContent;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileContent = memoryStream.ToArray();
                }

                var createDocumentDto = new CreateDocumentDto
                {
                    AccountId = accountId.Value,
                    FileName = file.FileName,
                    SizeBytes = file.Length,
                    ContentType = file.ContentType ?? "application/octet-stream",
                    FileContent = fileContent
                };

                var document = await _documentService.CreateDocumentAsync(createDocumentDto);

                // Save file to disk
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var filePath = Path.Combine(uploadsPath, $"{document.Id}_{file.FileName}");
                await System.IO.File.WriteAllBytesAsync(filePath, fileContent);

                _logger.LogInformation($"Document {document.Id} uploaded successfully for account {accountId}, saved to {filePath}");
                return Ok(document);
            }
            catch (InvalidOperationException ex)
            {
                // Check if this is a limit exceeded error
                if (ex.Message.Contains("limit exceeded", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning($"Upload rejected for account {accountId}: {ex.Message}");
                    var usage = await _usageService.GetCurrentUsageAsync(accountId.Value);
                    return StatusCode(403, new
                    {
                        error = "LIMIT_EXCEEDED",
                        message = ex.Message,
                        currentUsage = usage,
                        upgradeRequired = true
                    });
                }

                _logger.LogWarning($"Upload failed for account {accountId}: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error during upload for account {accountId}");
                return StatusCode(500, new { error = "An error occurred while uploading the file", details = ex.Message });
            }
        }

        [HttpGet("account/{accountId}/usage")]
        public async Task<ActionResult<UsageDto>> GetAccountUsage(Guid accountId)
        {
            try
            {
                var usage = await _usageService.GetCurrentUsageAsync(accountId);
                return Ok(usage);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDocument(Guid id)
        {
            try
            {
                await _documentService.DeleteDocumentAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}/download")]
        public async Task<ActionResult> DownloadDocument(Guid id)
        {
            try
            {
                var document = await _documentService.GetDocumentByIdAsync(id);
                if (document == null)
                {
                    _logger.LogWarning($"Document {id} not found for download");
                    return NotFound(new { error = "Document not found" });
                }

                // Get the file path
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                var filePath = Path.Combine(uploadsPath, $"{id}_{document.FileName}");

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning($"File not found at path: {filePath}. This document was likely uploaded before file storage was implemented.");

                    // Return a placeholder file with error message
                    var errorMessage = $"File content not available.\n\nThis document ({document.FileName}) was uploaded before the file storage system was implemented.\nOnly the document metadata is available.\n\nDocument ID: {id}\nUpload Date: {document.CreatedAt}\nSize: {document.SizeBytes} bytes";
                    var errorBytes = System.Text.Encoding.UTF8.GetBytes(errorMessage);

                    return File(errorBytes, "text/plain", $"ERROR_{document.FileName}.txt");
                }

                // Try to get file from cache first
                var cacheKey = $"file_content_{id}";
                byte[] fileBytes;

                if (_cache.TryGetValue<byte[]>(cacheKey, out var cachedBytes))
                {
                    _logger.LogInformation($"Returning cached file content for document {id}");
                    fileBytes = cachedBytes!;
                }
                else
                {
                    fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                    // Cache small files only (< 5MB)
                    if (fileBytes.Length < 5 * 1024 * 1024)
                    {
                        _cache.Set(cacheKey, fileBytes, TimeSpan.FromMinutes(10));
                        _logger.LogInformation($"Cached file content for document {id} ({fileBytes.Length} bytes) for 10 minutes");
                    }
                }

                _logger.LogInformation($"Successfully downloading document {id}: {document.FileName} ({fileBytes.Length} bytes)");
                return File(fileBytes, document.ContentType, document.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading document {id}");
                return StatusCode(500, new { error = "An error occurred while downloading the file" });
            }
        }
    }
}