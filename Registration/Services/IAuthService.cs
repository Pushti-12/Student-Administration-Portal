using Registration.DTOs.Auth;

namespace Registration.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);

        Task<bool> RegisterAsync(RegisterRequest request);

        Task LogoutAsync();

        Task<bool> ForgotPasswordAsync(
            ForgotPasswordRequest request,
            string resetLink);

        Task<bool> ResetPasswordAsync(
            ResetPasswordRequest request);
    }
}