using AccessControlApi.Domain.Entities;

namespace AccessControlApi.Domain.ApplicationInterfaces;

public interface IAuthorizationService
{
    Task<bool> HasAccessAsync(int userId, string requiredRole);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> RefreshToken(RefreshTokenRequest request);


}
