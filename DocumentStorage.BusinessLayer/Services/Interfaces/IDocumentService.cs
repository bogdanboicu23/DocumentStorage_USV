using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentStorage.Shared.DTOs.Document;

namespace DocumentStorage.BusinessLayer.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentDto>> GetAllDocumentsAsync();
        Task<IEnumerable<DocumentDto>> GetDocumentsByAccountIdAsync(Guid accountId);
        Task<DocumentDto?> GetDocumentByIdAsync(Guid id);
        Task<DocumentDto> CreateDocumentAsync(CreateDocumentDto createDocumentDto);
        Task<DocumentDto> UpdateDocumentAsync(UpdateDocumentDto updateDocumentDto);
        Task DeleteDocumentAsync(Guid id);
        Task<bool> CanDeleteDocumentAsync(Guid id);
        Task<long> GetAccountStorageUsageAsync(Guid accountId);
        Task<int> GetAccountDocumentCountAsync(Guid accountId);
    }
}