using System;

namespace DocumentStorage.Server.DTOs.Account
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string AccountType { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}