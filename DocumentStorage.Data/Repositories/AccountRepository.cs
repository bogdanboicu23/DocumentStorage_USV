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

        public async Task<Account?> GetAccountByUserIdAsync(Guid userId)
        {
            return await _context.AccountUsers
                .Where(au => au.UserId == userId)
                .Select(au => au.Account)
                .FirstOrDefaultAsync();
        }

        public async Task<Account> AddAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task AddAccountUserAsync(AccountUser accountUser)
        {
            _context.AccountUsers.Add(accountUser);
            await _context.SaveChangesAsync();
        }
    }
}