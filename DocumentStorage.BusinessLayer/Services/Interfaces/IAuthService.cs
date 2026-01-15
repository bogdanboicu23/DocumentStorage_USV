using DocumentStorage.Shared.DTOs.Auth;

namespace DocumentStorage.BusinessLayer.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
        Task<Guid?> GetUserAccountIdAsync(Guid userId);
    }
}