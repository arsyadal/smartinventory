using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartInventory.Application.Common;
using SmartInventory.Application.DTOs.Auth;
using SmartInventory.Application.Interfaces.Repositories;
using SmartInventory.Application.Interfaces.Services;

namespace SmartInventory.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepo, IOptions<JwtSettings> jwtOptions)
    {
        _userRepo = userRepo;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<ApiResponse<TokenDto>> LoginAsync(LoginDto dto)
    {
        return await ValidateUserAsync(dto);
    }

    public async Task<ApiResponse<TokenDto>> ValidateUserAsync(LoginDto dto)
    {
        var user = await _userRepo.GetByUsernameAsync(dto.Username);
        if (user == null)
            return ApiResponse<TokenDto>.Fail("Invalid username or password.", "INVALID_CREDENTIALS");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return ApiResponse<TokenDto>.Fail("Invalid username or password.", "INVALID_CREDENTIALS");

        var expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
        var token = GenerateJwtToken(user.Username, user.Role.ToString(), expiry);

        return ApiResponse<TokenDto>.Ok(new TokenDto
        {
            Token = token,
            Expiry = expiry,
            Username = user.Username,
            Role = user.Role.ToString()
        });
    }

    private string GenerateJwtToken(string username, string role, DateTime expiry)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiry,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
