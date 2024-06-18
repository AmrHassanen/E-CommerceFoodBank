using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using FoodBank.CORE.Dtos;
using FoodBank.CORE.Entities;
using FoodBank.CORE.Interfaces;
using FoodBank.EF.Helpers;
using FoodBank.CORE.Authentications;
using Microsoft.AspNetCore.Http;

namespace CaptionGenerator.EF.Repositories
{
    public class AuthUser : IAuthUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly ILogger<AuthUser> _logger;
        private readonly IPhotoService _photoService;



        public AuthUser(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, ILogger<AuthUser> logger, IHttpContextAccessor httpContextAccessor, IPhotoService photoService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _photoService = photoService;
        }
        public async Task<FoodBankUser> RegisterAsync(RegisterModelDto registerModelDto)
        {
            if (await _userManager.FindByEmailAsync(registerModelDto.Email) != null)
            {
                return new FoodBankUser { Message = "Email Is Already Register!" };
            }
            if (await _userManager.FindByEmailAsync(registerModelDto.UserName) != null)
            {
                return new FoodBankUser { Message = "UserName Is Already Register!" };
            }
            var User = new ApplicationUser
            {
                PasswordHash = registerModelDto.Password,
                Email = registerModelDto.Email,
                UserName = registerModelDto.UserName,
            };
            var Result = await _userManager.CreateAsync(User, registerModelDto.Password);
            if (!Result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in Result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new FoodBankUser { Message = errors };
            }
            await _userManager.AddToRoleAsync(User, "User");
            var jwtSecurityToken = await CreateJwtToken(User);
            return new FoodBankUser
            {
                Email = User.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = User.UserName,
            };

        }

        public async Task<FoodBankUser> GetTokenAsync(GetTokenRequstDto getTokenRequestDto)
        {
            var authUser = new FoodBankUser();

            var user = await _userManager.FindByEmailAsync(getTokenRequestDto.Email); // Find by email instead of username
            if (user == null || !await _userManager.CheckPasswordAsync(user, getTokenRequestDto.Passward))
            {
                return new FoodBankUser { Message = "Email or Password is incorrect" }; // Update error message
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authUser.IsAuthenticated = true;
            authUser.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authUser.Email = user.Email;
            authUser.ExpiresOn = jwtSecurityToken.ValidTo;
            authUser.UserName = user.UserName;
            authUser.Roles = rolesList.ToList();

            return authUser;
        }

        public async Task<string> AddRoleAsync(RoleModelDto roleModel)
        {
            var user = await _userManager.FindByIdAsync(roleModel.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(roleModel.Role))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, roleModel.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, roleModel.Role);

            return result.Succeeded ? string.Empty : "Sonething went wrong";
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim(ClaimTypes.Role, role));

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim("uid", user.Id)
    }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays((double)_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        public async Task<bool> ForgetPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // Include the token in the email message
                    var resetPasswordLink = $"{Uri.EscapeDataString(token)}&email={user.Email}";
                    var messageContent = $"Click <a href='{resetPasswordLink}'>here</a> to reset your password. Your token: {token}";

                    // TODO: Replace the following lines with your email sending logic
                    var message = new MailMessage
                    {
                        From = new MailAddress("amroyasser55555@gmail.com", "Tecical Team"),
                        Subject = "Forget Password Link",
                        Body = messageContent,
                        IsBodyHtml = true
                    };

                    // Set the recipient's email address
                    message.To.Add(new MailAddress(user.Email));

                    // Use your SMTP server details
                    using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                    {
                        smtpClient.Port = 587;
                        smtpClient.Credentials = new NetworkCredential("amroyasser55555@gmail.com", "vkko ppmn sihc lupe");
                        smtpClient.EnableSsl = true;

                        // Send the email
                        smtpClient.Send(message);
                    }

                    return true; // Password reset link sent successfully
                }

                return false; // User not found or couldn't send the link to the email
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ForgetPasswordAsync: {ex}");
                throw; // You may handle this exception as needed, e.g., log and return a custom response
            }
        }


        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

                if (user == null)
                {
                    // User with the provided email does not exist
                    return false;
                }

                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.ResetToken, resetPasswordDto.NewPassword);

                if (result.Succeeded)
                {
                    // Password reset successful
                    return true;
                }

                // Password reset failed
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ResetPasswordAsync: {ex}");
                throw; // You may handle this exception as needed, e.g., log and return a custom response
            }
        }

        public async Task<UserProfileDto> GetUserProfileAsync()
        {
            var userEmailClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmailClaim))
            {
                return null; // Email claim not found or empty
            }

            var user = await _userManager.FindByEmailAsync(userEmailClaim);

            if (user == null)
            {
                return null; // User not found
            }

            // Map user data to UserProfileDto object
            var userProfileDto = new UserProfileDto
            {
                Email = user.Email,
                Username = user.UserName,
                ImageUrl = user.ImageUrl,
                // Add other properties as needed
            };

            return userProfileDto;
        }


        public async Task<bool> UpdateUserProfileAsync(UpdateProfileDto updateProfileDto)
        {
            try
            {
                var userEmailClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

                if (string.IsNullOrEmpty(userEmailClaim))
                {
                    throw new Exception("Email claim not found or empty");
                }

                var user = await _userManager.FindByEmailAsync(userEmailClaim);

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                var hasNewImageUrl = updateProfileDto.ImageUrl is not null;

                if (hasNewImageUrl)
                {
                    var imageUploadResult = await _photoService.AddPhotoAsync(updateProfileDto.ImageUrl!);
                    user.ImageUrl = imageUploadResult.SecureUrl.AbsoluteUri;
                }
                // Update user profile properties based on the received DTO
                user.UserName = updateProfileDto.Username;
                user.Email = updateProfileDto.Email;
                // Add other properties as needed

                var result = await _userManager.UpdateAsync(user);

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user profile: {ex.Message}");
                throw; // Re-throw the exception for the caller to handle
            }
        }
    }
}