using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        string GenerateJwtToken(int userId, string username);
    }
}
