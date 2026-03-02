using WebApplicationReact.Models.DTOs;
namespace WebApplicationReact.Services.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
