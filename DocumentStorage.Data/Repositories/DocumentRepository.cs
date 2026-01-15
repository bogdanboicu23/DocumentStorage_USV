 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.Data.Models;
using DocumentStorage.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.Data.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DocumentStorageDbContext _context;

        public DocumentRepository(DocumentStorageDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _context.Documents
                .Include(d => d.Account)
                .Where(d => !d.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Documents
                .Include(d => d.Account)
                .Where(d => d.AccountId == accountId && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<Document?> GetByIdAsync(Guid id)
        {
            return await _context.Documents
                .Include(d => d.Account)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Document> AddAsync(Document document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<Document> UpdateAsync(Document document)
        {
            _context.Documents.Update(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task DeleteAsync(Guid id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document != null)
            {
                document.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Documents.AnyAsync(d => d.Id == id);
        }

        public async Task<long> GetAccountStorageUsageAsync(Guid accountId)
        {
            return await _context.Documents
                .Where(d => d.AccountId == accountId && !d.IsDeleted)
                .SumAsync(d => d.SizeBytes);
        }

        public async Task<int> GetAccountDocumentCountAsync(Guid accountId)
        {
            return await _context.Documents
                .Where(d => d.AccountId == accountId && !d.IsDeleted)
                .CountAsync();
        }
    }
}