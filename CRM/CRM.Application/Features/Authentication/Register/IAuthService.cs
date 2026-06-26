using CRM.Application.Features.Authentication.Register;

namespace CRM.Application.Features.Authentication.Register;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
}