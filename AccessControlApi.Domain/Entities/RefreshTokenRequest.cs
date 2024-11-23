namespace AccessControlApi.Domain.Entities;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; }
    public string Token { get; set; }
}
