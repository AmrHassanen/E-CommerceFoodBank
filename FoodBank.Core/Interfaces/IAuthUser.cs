using FoodBank.CORE.Authentications;
using FoodBank.CORE.Dtos;

namespace FoodBank.CORE.Interfaces
{
    public interface IAuthUser
    {
        Task<FoodBankUser> RegisterAsync(RegisterModelDto registerModelDto);
        Task<FoodBankUser> GetTokenAsync(GetTokenRequstDto getTokenRequstDto);
        Task<string> AddRoleAsync(RoleModelDto roleModel);
        Task<bool> ForgetPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<UserProfileDto> GetUserProfileAsync();
        Task<bool> UpdateUserProfileAsync(UpdateProfileDto updateProfileDto);
    }
}