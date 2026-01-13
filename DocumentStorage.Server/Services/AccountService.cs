using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.Data.Models;
using DocumentStorage.Data.Repositories.Interfaces;
using DocumentStorage.Server.DTOs.Account;
using DocumentStorage.Server.Services.Interfaces;

namespace DocumentStorage.Server.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAccountsAsync();
            return accounts.Select(MapToDto);
        }

        public async Task<AccountDto> GetAccountByIdAsync(Guid id)
        {
            var account = await _accountRepository.GetAccountByIdAsync(id);
            return account != null ? MapToDto(account) : null;
        }

        private AccountDto MapToDto(Account account)
        {
            return new AccountDto
            {
                Id = account.Id,
                AccountType = account.AccountType,
                IsActive = account.IsActive
            };
        }
    }
}