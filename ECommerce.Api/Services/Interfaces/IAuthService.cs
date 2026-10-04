using ECommerce.Api.Dtos.Auth;

namespace ECommerce.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto dto);
        Task<string> VerifyEmailAsync(string userId, string token);
        Task<string> ForgotPasswordAsync(string email);
        Task<string> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
