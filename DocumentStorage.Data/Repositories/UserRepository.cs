using System;
using System.Threading.Tasks;
using DocumentStorage.Data.Models;
using DocumentStorage.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DocumentStorageDbContext _context;

        public UserRepository(DocumentStorageDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var normalizedEmail = email?.Trim().ToLowerInvariant();
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var normalizedEmail = email?.Trim().ToLowerInvariant();
            return await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        }
    }
}