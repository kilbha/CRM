using CRM.Application.Features.Authentication.Register;
using CRM.Application.Features.Authentication.Login;

namespace CRM.Application.Features.Authentication.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
}