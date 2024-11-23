using AccessControlApi.Domain.ApplicationInterfaces;
using AccessControlApi.Domain.Entities;
using AccessControlApi.Domain.InfrastructureInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccessControlApi.Application.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly string _jwtSecret;
    private readonly int _tokenExpirationDays;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public AuthorizationService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _jwtSecret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT secret not configured");
        _tokenExpirationDays = int.Parse(configuration["Jwt:ExpirationDays"] ?? "7");
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public async Task<bool> HasAccessAsync(int userId, string requiredRole)
    {
        if (userId <= 0) throw new ArgumentException("Invalid user ID", nameof(userId));
        if (string.IsNullOrWhiteSpace(requiredRole)) throw new ArgumentException("Role cannot be empty", nameof(requiredRole));

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        var role = await _roleRepository.GetRoleByIdAsync(user.RoleId);
        if (role == null) return false;

        return role.Name.Equals(requiredRole, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Username)) throw new ArgumentException("Username cannot be empty", nameof(request));
        if (string.IsNullOrWhiteSpace(request.Password)) throw new ArgumentException("Password cannot be empty", nameof(request));

        var user = await _userRepository.GetUserByCredentialsAsync(request.Username, request.Password);
        if (user == null)
        {
            return new AuthResult { IsSuccessful = false, ErrorMessage = "Invalid credentials" };
        }

        try
        {
            var token = GenerateToken(user);
            return new AuthResult
            {
                IsSuccessful = true,
                Token = token,
                UserId = user.Id
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                IsSuccessful = false,
                ErrorMessage = "Error generating token"
            };
        }
    }

    public async Task<AuthResult> RefreshToken(RefreshTokenRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Token)) throw new ArgumentException("Token cannot be empty", nameof(request));

        try
        {
            var validationParameters = GetTokenValidationParameters();
            var principal = _tokenHandler.ValidateToken(request.Token, validationParameters, out var validatedToken);
            var userId = int.Parse(principal.Identity?.Name ?? "0");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new AuthResult { IsSuccessful = false, ErrorMessage = "User not found" };
            }

            var newToken = GenerateToken(user);
            return new AuthResult
            {
                IsSuccessful = true,
                Token = newToken,
                UserId = user.Id
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                IsSuccessful = false,
                ErrorMessage = "Invalid token"
            };
        }
    }

    private string GenerateToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(_jwtSecret);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(_tokenExpirationDays),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    private TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    }
}