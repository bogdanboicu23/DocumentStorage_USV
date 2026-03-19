using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentStorage.Shared.DTOs.Account;

namespace DocumentStorage.BusinessLayer.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountDto>> GetAllAccountsAsync();
        Task<AccountDto?> GetAccountByIdAsync(Guid id);
    }
}