using Microsoft.AspNetCore.Mvc;
using TmsApi.Domain.Entities;
using TmsApi.Application.Services;
using TmsApi.Application.Dtos;
using TmsApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<TmsUser> _userManager; private readonly RoleManager<IdentityRole> _roleManager;
    public AuthController(UserManager<TmsUser> userManager,
    RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public record RegisterRequest(string Email,
           string Password,
           string FirstName,
           string LastName,
           string Role);
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existingUser = await
_userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Ok(new { message = "Registration request received." });
        }
        var user = new TmsUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        var result = await _userManager.CreateAsync(user,
request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { errors });
        }
        if (!await _roleManager.RoleExistsAsync(request.Role))
        {
            await _roleManager.CreateAsync(new IdentityRole(request.Role));
        }
        await _userManager.AddToRoleAsync(user, request.Role);
        return Ok(new { message = "Registration successful." });
    }
    public record LoginRequest(string Email, string Password);
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized(new { detail = "Invalid credentials." });
        }
        if (await _userManager.IsLockedOutAsync(user))
        {
            return StatusCode(423, new { detail = "Account locked due to multiple failed login attempts. Try again in 15 minutes." });
        }
        var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!validPassword)
        {
            await _userManager.AccessFailedAsync(user);
            return Unauthorized(new { detail = "Invalid credentials." });
        }
        await _userManager.ResetAccessFailedCountAsync(user);
        return Ok(new
        {
            userId = user.Id,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName
        });
    }
}









// using Microsoft.AspNetCore.Mvc;
// using TmsApi.Domain.Entities;
// using TmsApi.Application.Services;
// using TmsApi.Application.Dtos;

// [ApiController]
// [Route("api/{version:apiVersion}/auth")]
// public class AuthController : ControllerBase
// {

//     [HttpPost("login")]
//     public IActionResult Login(
//               [FromBody] LoginRequest request,
//               [FromServices] IWebHostEnvironment env)
//     {
//         if (request.Username == "admin" && request.Password == "Password123!")
//         {
//             var dummyJwt = "header.payload.signature-demo-token";
//             Response.Cookies.Append("tms_auth", dummyJwt, new CookieOptions
//             {
//                 HttpOnly = true,
//                 Secure = !env.IsDevelopment(),
//                 SameSite = SameSiteMode.Strict,
//                 Expires = DateTimeOffset.UtcNow.AddHours(2)
//             });
//             return Ok(new UserProfileDto("System Admin", "Admin"));
//         }
//         return Unauthorized(new { detail = "Invalid username or password." });
//     }
//     [HttpGet("me")]
//     public IActionResult GetCurrentUser()
//     {
//         // Inspect cookie attached automatically by the browser on cross-origin requests
//         if (Request.Cookies.TryGetValue("tms_auth", out _))
//         {
//             return Ok(new UserProfileDto("System Admin",
//             "Admin"));
//         }
//         return Unauthorized(new { detail = "Session expired or missing authentication cookie." });
//     }
// }



