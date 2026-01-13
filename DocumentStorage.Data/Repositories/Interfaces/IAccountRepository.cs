using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentStorage.Data.Models;

namespace DocumentStorage.Data.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Account> GetAccountByIdAsync(Guid id);
    }
}