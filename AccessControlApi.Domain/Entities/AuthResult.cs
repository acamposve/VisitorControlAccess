namespace AccessControlApi.Domain.Entities;

public class AuthResult
{
    public bool IsSuccessful { get; set; }
    public string? Token { get; set; }
    public string? ErrorMessage { get; set; }
    public int UserId { get; set; }
}
