using SmartInventory.Application.Common;
using SmartInventory.Application.DTOs.Auth;

namespace SmartInventory.Application.Interfaces.Services;

public interface IAuthService
{
    Task<ApiResponse<TokenDto>> LoginAsync(LoginDto dto);
    Task<ApiResponse<TokenDto>> ValidateUserAsync(LoginDto dto);
}
