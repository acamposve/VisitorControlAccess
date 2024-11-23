using AccessControlApi.Domain.Entities;

namespace AccessControlApi.Domain.ApplicationInterfaces;

public interface IAuthorizationService
{
    Task<AuthResult> RefreshToken(RefreshTokenRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
}
