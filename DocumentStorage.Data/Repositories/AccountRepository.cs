using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.Data.Models;
using DocumentStorage.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DocumentStorageDbContext _context;

        public AccountRepository(DocumentStorageDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _context.Accounts
                .Where(a => a.IsActive)
                .OrderBy(a => a.Id)
                .ToListAsync();
        }

        public async Task<Account> GetAccountByIdAsync(Guid id)
        {
            return await _context.Accounts.FindAsync(id);
        }
    }
}