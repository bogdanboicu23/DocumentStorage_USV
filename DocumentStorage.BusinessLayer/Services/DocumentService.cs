using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.Data.Models;
using DocumentStorage.Data.Repositories.Interfaces;
using DocumentStorage.Shared.DTOs.Document;
using DocumentStorage.BusinessLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentStorage.BusinessLayer.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUsageService _usageService;
        private readonly ILogger<DocumentService> _logger;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public DocumentService(
            IDocumentRepository documentRepository,
            IAccountRepository accountRepository,
            ISubscriptionRepository subscriptionRepository,
            IUsageService usageService,
            ILogger<DocumentService> logger,
            IMemoryCache cache)
        {
            _documentRepository = documentRepository;
            _accountRepository = accountRepository;
            _subscriptionRepository = subscriptionRepository;
            _usageService = usageService;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<DocumentDto>> GetAllDocumentsAsync()
        {
            const string cacheKey = "all_documents";

            if (_cache.TryGetValue<IEnumerable<DocumentDto>>(cacheKey, out var cachedDocuments))
            {
                _logger.LogInformation("Returning cached documents list");
                return cachedDocuments!;
            }

            var documents = await _documentRepository.GetAllAsync();
            var documentDtos = documents.Select(d => new DocumentDto
            {
                Id = d.Id,
                AccountId = d.AccountId,
                FileName = d.FileName,
                SizeBytes = d.SizeBytes,
                ContentType = d.ContentType,
                CreatedAt = d.CreatedAt,
                IsDeleted = d.IsDeleted,
                AccountName = $"Account {d.AccountId}"
            }).ToList();

            _cache.Set(cacheKey, documentDtos, _cacheExpiration);
            _logger.LogInformation($"Cached {documentDtos.Count} documents for {_cacheExpiration.TotalMinutes} minutes");

            return documentDtos;
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentsByAccountIdAsync(Guid accountId)
        {
            var documents = await _documentRepository.GetByAccountIdAsync(accountId);
            return documents.Select(d => new DocumentDto
            {
                Id = d.Id,
                AccountId = d.AccountId,
                FileName = d.FileName,
                SizeBytes = d.SizeBytes,
                ContentType = d.ContentType,
                CreatedAt = d.CreatedAt,
                IsDeleted = d.IsDeleted,
                AccountName = $"Account {d.AccountId}"
            });
        }

        public async Task<DocumentDto?> GetDocumentByIdAsync(Guid id)
        {
            var cacheKey = $"document_{id}";

            if (_cache.TryGetValue<DocumentDto>(cacheKey, out var cachedDocument))
            {
                _logger.LogInformation($"Returning cached document {id}");
                return cachedDocument;
            }

            var document = await _documentRepository.GetByIdAsync(id);

            if (document == null)
                return null;

            var documentDto = new DocumentDto
            {
                Id = document.Id,
                AccountId = document.AccountId,
                FileName = document.FileName,
                SizeBytes = document.SizeBytes,
                ContentType = document.ContentType,
                CreatedAt = document.CreatedAt,
                IsDeleted = document.IsDeleted,
                AccountName = $"Account {document.AccountId}"
            };

            _cache.Set(cacheKey, documentDto, _cacheExpiration);
            _logger.LogInformation($"Cached document {id} for {_cacheExpiration.TotalMinutes} minutes");

            return documentDto;
        }

        public async Task<DocumentDto> CreateDocumentAsync(CreateDocumentDto createDocumentDto)
        {
            _logger.LogInformation($"Creating document for account {createDocumentDto.AccountId}, File: {createDocumentDto.FileName}, Size: {createDocumentDto.SizeBytes} bytes");

            var account = await _accountRepository.GetAccountByIdAsync(createDocumentDto.AccountId);
            if (account == null)
            {
                _logger.LogWarning($"Account with ID {createDocumentDto.AccountId} not found");
                throw new InvalidOperationException($"Account with ID {createDocumentDto.AccountId} not found.");
            }

            var subscriptions = await _subscriptionRepository.GetSubscriptionsByAccountAsync(createDocumentDto.AccountId);
            var subscription = subscriptions.FirstOrDefault(s => s.Status == "Active");

            if (subscription != null)
            {

                var storageLimit = subscription.Plan.PlanLimits
                    .FirstOrDefault(pl => pl.ResourceType == "storage");
                var documentCountLimit = subscription.Plan.PlanLimits
                    .FirstOrDefault(pl => pl.ResourceType == "documents");

                if (storageLimit != null)
                {
                    var currentUsage = await GetAccountStorageUsageAsync(createDocumentDto.AccountId);
                    var newUsageBytes = currentUsage + createDocumentDto.SizeBytes;


                    if (newUsageBytes > storageLimit.MaxValue)
                    {
                        _logger.LogWarning($"Storage limit exceeded for account {createDocumentDto.AccountId}. Usage: {newUsageBytes}/{storageLimit.MaxValue} bytes");
                        throw new InvalidOperationException("Storage limit exceeded for this account.");
                    }
                }

                if (documentCountLimit != null)
                {
                    var currentCount = await GetAccountDocumentCountAsync(createDocumentDto.AccountId);

                    _logger.LogInformation($"Document count check - Current: {currentCount}, Limit: {documentCountLimit.MaxValue}");

                    if (currentCount >= documentCountLimit.MaxValue)
                    {
                        _logger.LogWarning($"Document count limit exceeded for account {createDocumentDto.AccountId}. Count: {currentCount}/{documentCountLimit.MaxValue}");
                        throw new InvalidOperationException("Document count limit exceeded for this account.");
                    }
                }
            }
            else
            {
                _logger.LogInformation($"No active subscription found for account {createDocumentDto.AccountId}, using free tier limits");
            }

            var document = new Document
            {
                Id = Guid.NewGuid(),
                AccountId = createDocumentDto.AccountId,
                FileName = createDocumentDto.FileName,
                SizeBytes = createDocumentDto.SizeBytes,
                ContentType = createDocumentDto.ContentType,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _documentRepository.AddAsync(document);

            // Invalidate cache
            _cache.Remove("all_documents");
            _cache.Remove($"account_documents_{createDocumentDto.AccountId}");

            _logger.LogInformation($"Document {document.Id} created successfully for account {createDocumentDto.AccountId}, cache invalidated");

            return new DocumentDto
            {
                Id = document.Id,
                AccountId = document.AccountId,
                FileName = document.FileName,
                SizeBytes = document.SizeBytes,
                ContentType = document.ContentType,
                CreatedAt = document.CreatedAt,
                IsDeleted = document.IsDeleted,
                AccountName = $"Account {account.Id}"
            };
        }

        public async Task<DocumentDto> UpdateDocumentAsync(UpdateDocumentDto updateDocumentDto)
        {
            var document = await _documentRepository.GetByIdAsync(updateDocumentDto.Id);

            if (document == null)
                throw new InvalidOperationException($"Document with ID {updateDocumentDto.Id} not found.");

            document.FileName = updateDocumentDto.FileName;
            document.IsDeleted = updateDocumentDto.IsDeleted;

            await _documentRepository.UpdateAsync(document);

            return new DocumentDto
            {
                Id = document.Id,
                AccountId = document.AccountId,
                FileName = document.FileName,
                SizeBytes = document.SizeBytes,
                ContentType = document.ContentType,
                CreatedAt = document.CreatedAt,
                IsDeleted = document.IsDeleted,
                AccountName = $"Account {document.AccountId}"
            };
        }

        public async Task DeleteDocumentAsync(Guid id)
        {
            var exists = await _documentRepository.ExistsAsync(id);

            if (!exists)
                throw new InvalidOperationException($"Document with ID {id} not found.");

            // Get document to invalidate account cache
            var document = await _documentRepository.GetByIdAsync(id);

            await _documentRepository.DeleteAsync(id);

            // Invalidate cache
            _cache.Remove("all_documents");
            _cache.Remove($"document_{id}");
            if (document != null)
            {
                _cache.Remove($"account_documents_{document.AccountId}");
            }

            _logger.LogInformation($"Document {id} deleted and cache invalidated");
        }

        public async Task<bool> CanDeleteDocumentAsync(Guid id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            return document != null && !document.IsDeleted;
        }

        public async Task<long> GetAccountStorageUsageAsync(Guid accountId)
        {
            return await _documentRepository.GetAccountStorageUsageAsync(accountId);
        }

        public async Task<int> GetAccountDocumentCountAsync(Guid accountId)
        {
            return await _documentRepository.GetAccountDocumentCountAsync(accountId);
        }
    }
}