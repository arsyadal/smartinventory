namespace SmartInventory.Application.DTOs.Auth;

public class TokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
