using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DocumentStorage.BusinessLayer.Services.Interfaces;
using DocumentStorage.Data.Models;
using DocumentStorage.Data.Repositories.Interfaces;
using DocumentStorage.Shared.DTOs.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace DocumentStorage.BusinessLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    _logger.LogWarning($"Login attempt failed: User not found for email {loginDto.Email}");
                    return null;
                }

                // Verify password using BCrypt
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    _logger.LogWarning($"Login attempt failed: Invalid password for user {user.Email}");
                    return null;
                }

                var token = GenerateJwtToken(user.Id, user.Email, user.FirstName, user.LastName);

                _logger.LogInformation($"User {user.Email} logged in successfully");

                return new AuthResponseDto
                {
                    Token = token,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserId = user.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                throw;
            }
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning($"Registration failed: User already exists with email {registerDto.Email}");
                    return null;
                }

                // Create password hash using BCrypt
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                // Create new user
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    PasswordHash = passwordHash
                };

                var createdUser = await _userRepository.CreateAsync(user);

                // Create account for the user
                var account = new Account
                {
                    Id = Guid.NewGuid(),
                    AccountType = "Personal",
                    IsActive = true
                };

                var createdAccount = await _accountRepository.AddAsync(account);

                // Link user to account
                var accountUser = new AccountUser
                {
                    AccountId = createdAccount.Id,
                    UserId = createdUser.Id,
                    Role = "Owner"
                };

                await _accountRepository.AddAccountUserAsync(accountUser);

                var token = GenerateJwtToken(createdUser.Id, createdUser.Email, createdUser.FirstName, createdUser.LastName);

                _logger.LogInformation($"User {createdUser.Email} registered successfully");

                return new AuthResponseDto
                {
                    Token = token,
                    Email = createdUser.Email,
                    FirstName = createdUser.FirstName,
                    LastName = createdUser.LastName,
                    UserId = createdUser.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                throw;
            }
        }

        public async Task<Guid?> GetUserAccountIdAsync(Guid userId)
        {
            var account = await _accountRepository.GetAccountByUserIdAsync(userId);
            return account?.Id;
        }

        private string GenerateJwtToken(Guid userId, string email, string firstName, string lastName)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.GivenName, firstName),
                new Claim(ClaimTypes.Surname, lastName),
                new Claim(ClaimTypes.Name, $"{firstName} {lastName}")
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}