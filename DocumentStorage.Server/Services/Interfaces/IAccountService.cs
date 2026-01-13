using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentStorage.Server.DTOs.Account;

namespace DocumentStorage.Server.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountDto>> GetAllAccountsAsync();
        Task<AccountDto> GetAccountByIdAsync(Guid id);
    }
}