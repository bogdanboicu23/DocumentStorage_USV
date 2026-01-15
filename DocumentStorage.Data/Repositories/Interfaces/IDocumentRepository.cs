using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentStorage.Data.Models;

namespace DocumentStorage.Data.Repositories.Interfaces
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> GetAllAsync();
        Task<IEnumerable<Document>> GetByAccountIdAsync(Guid accountId);
        Task<Document?> GetByIdAsync(Guid id);
        Task<Document> AddAsync(Document document);
        Task<Document> UpdateAsync(Document document);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<long> GetAccountStorageUsageAsync(Guid accountId);
        Task<int> GetAccountDocumentCountAsync(Guid accountId);
    }
}