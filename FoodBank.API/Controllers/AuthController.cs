
using FoodBank.CORE.Dtos;
using FoodBank.CORE.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthUser _authUserService;

    public AuthController(IAuthUser authUserService)
    {
        _authUserService = authUserService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModelDto registerModelDto)
    {
        var result = await _authUserService.RegisterAsync(registerModelDto);

        if (result.IsAuthenticated)
        {
            // Set the token in cookies 
            Response.Cookies.Append("accessToken", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Set to true if using HTTPS 
                SameSite = SameSiteMode.Strict // Adjust as needed 
            });

            return Ok(result);
        }
        else
        {
            return BadRequest(new { Message = result.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] GetTokenRequstDto getTokenRequstDto)
    {
        var result = await _authUserService.GetTokenAsync(getTokenRequstDto);

        if (result.IsAuthenticated)
        {
            // Set the token in cookies 
            Response.Cookies.Append("accessToken", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Set to true if using HTTPS 
                SameSite = SameSiteMode.Strict // Adjust as needed 
            });

            return Ok(result);
        }
        else
        {
            return Unauthorized(new { Message = result.Message });
        }
    }

    [HttpPost("addrole")]
    [Authorize(Roles = "Admin")] // Only accessible to users with the "Admin" role
    public async Task<IActionResult> AddRole([FromBody] RoleModelDto roleModel)
    {
        var result = await _authUserService.AddRoleAsync(roleModel);

        if (string.IsNullOrEmpty(result))
        {
            return Ok(new { Message = "Role added successfully." });
        }
        else
        {
            return BadRequest(new { Message = result });
        }
    }

    [HttpPost("forgetpassword")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto forgetPasswordDto)
    {
        var result = await _authUserService.ForgetPasswordAsync(forgetPasswordDto.Email);

        if (result)
        {
            return Ok(new { Message = "Password reset link sent successfully." });
        }
        else
        {
            return BadRequest(new { Message = "Failed to send the reset link." });
        }
    }

    [HttpPost("resetpassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        var result = await _authUserService.ResetPasswordAsync(resetPasswordDto);

        if (result)
        {
            return Ok(new { Message = "Password reset successfully." });
        }
        else
        {
            return BadRequest(new { Message = "Failed to reset the password." });
        }
    }
    [HttpGet("profile")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetUserProfile()
    {
        // Retrieve user data from the current context (e.g., using User.Identity.Name)
        var userEmail = User.Identity.Name; // Assuming email is used for authentication

        // Retrieve user profile data from your service or repository
        var userProfile = await _authUserService.GetUserProfileAsync();

        if (userProfile != null)
        {
            return Ok(userProfile); // Return user profile data
        }
        else
        {
            return NotFound(new { Message = "User profile not found." });
        }
    }

    [HttpPut("profile")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> UpdateUserProfile([FromForm] UpdateProfileDto updateProfileDto)
    {
        // Retrieve user data from the current context (e.g., using User.Identity.Name)
        var userEmail = User.Identity.Name; // Assuming email is used for authentication

        // Update user profile data based on the received parameters
        var result = await _authUserService.UpdateUserProfileAsync(updateProfileDto);

        if (result)
        {
            return Ok(new { Message = "Profile updated successfully." });
        }
        else
        {
            return BadRequest(new { Message = "Failed to update profile." });
        }
    }
}