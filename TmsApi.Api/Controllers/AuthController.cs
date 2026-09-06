using Microsoft.AspNetCore.Mvc;
using TmsApi.Domain.Entities;
using TmsApi.Application.Services;
using TmsApi.Application.Dtos;
using TmsApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TmsApi.Infrastructure.Persistence;
using TmsApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;



namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<TmsUser> _userManager; private readonly RoleManager<IdentityRole> _roleManager; private readonly TmsDbContext _context;
    private readonly TokenService _tokenService;
    public AuthController(UserManager<TmsUser> userManager, RoleManager<IdentityRole> roleManager, TmsDbContext context,
    TokenService tokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _tokenService = tokenService;
    }
    [EnableRateLimiting("AuthLimiter")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email); if (user == null) return Unauthorized(new { detail = "Invalid credentials." });
        if (await _userManager.IsLockedOutAsync(user))
        {
            return StatusCode(423, new { detail = "Account locked due to multiple failed login attempts." });
        }
        var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!validPassword)
        {
            await _userManager.AccessFailedAsync(user); return Unauthorized(new { detail = "Invalid credentials." });
        }
        await _userManager.ResetAccessFailedCountAsync(user); var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateJwt(user, roles);
        // Issue initial Refresh Token
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString("N"),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false
        };
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return Ok(new
        {
            accessToken,
            refreshToken = refreshToken.Token
        });
    }
    public record RefreshRequest(string RefreshToken);
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var storedToken = await _context.RefreshTokens
.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (storedToken == null)
        {
            return Unauthorized(new { detail = "Invalid refreshtoken." });
        }
        // Theft Detection: If an ALREADY-USED token is submitted, revoke ALL tokens for this user!
        if (storedToken.IsUsed)
        {
            var userTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == storedToken.UserId)
                .ToListAsync();
            foreach (var t in userTokens)
            {
                t.IsRevoked = true;
            }
            await _context.SaveChangesAsync();
            return Unauthorized(new { detail = "Token theft detected. All user sessions revoked." });
        }
        if (storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            return Unauthorized(new { detail = "Refresh token expired or revoked." });
        }
        // Mark current token as used
        storedToken.IsUsed = true;
        // Issue brand-new Refresh Token pair
        var newRefreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString("N"),
            UserId = storedToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false
        }; _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();
        var user = await _userManager.FindByIdAsync(storedToken.UserId);

        var roles = await _userManager.GetRolesAsync(user!);
        var newAccessToken = _tokenService.GenerateJwt(user!, roles);
        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken.Token
        });
    }
public record RegisterRequest(
    string Email,
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

}


























// namespace TmsApi.Api.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class AuthController : ControllerBase
// {
//     private readonly UserManager<TmsUser> _userManager; 
//     private readonly RoleManager<IdentityRole> _roleManager;
//     private readonly TmsDbContext _context;
//     private readonly TokenService _tokenService;

//     public AuthController(UserManager<TmsUser> userManager,
//     RoleManager<IdentityRole> roleManager)
//     {
//         _userManager = userManager;
//         _roleManager = roleManager;
//             _context = context;
//         _tokenService = tokenService;
//     }
//     public record RegisterRequest(string Email,
//            string Password,
//            string FirstName,
//            string LastName,
//            string Role);
//     [HttpPost("register")]
//     public async Task<IActionResult> Register([FromBody] RegisterRequest request)
//     {
//         var existingUser = await
// _userManager.FindByEmailAsync(request.Email);
//         if (existingUser != null)
//         {
//             return Ok(new { message = "Registration request received." });
//         }
//         var user = new TmsUser
//         {
//             UserName = request.Email,
//             Email = request.Email,
//             FirstName = request.FirstName,
//             LastName = request.LastName
//         };
//         var result = await _userManager.CreateAsync(user,
// request.Password);
//         if (!result.Succeeded)
//         {
//             var errors = result.Errors.Select(e => e.Description);
//             return BadRequest(new { errors });
//         }
//         if (!await _roleManager.RoleExistsAsync(request.Role))
//         {
//             await _roleManager.CreateAsync(new IdentityRole(request.Role));
//         }
//         await _userManager.AddToRoleAsync(user, request.Role);
//         return Ok(new { message = "Registration successful." });
//     }
//     public record LoginRequest(string Email, string Password);


//       [HttpPost("login")]
// public async Task<IActionResult> Login([FromBody] LoginRequest request)
// {
// var user = await _userManager.FindByEmailAsync(request.Email); if (user == null) return Unauthorized(new { detail = "Invalid credentials."});
// if (await _userManager.IsLockedOutAsync(user)) {
// return StatusCode(423, new { detail = "Account locked due to multiple failed login attempts." });
// }
// var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
// if (!validPassword)
// {
// await _userManager.AccessFailedAsync(user); return Unauthorized(new { detail = "Invalid credentials." });
//         }
// await _userManager.ResetAccessFailedCountAsync(user); var roles = await _userManager.GetRolesAsync(user);
// var accessToken = _tokenService.GenerateJwt(user, roles);
//         // Issue initial Refresh Token
// var refreshToken = new RefreshToken
// {
// Token = Guid.NewGuid().ToString("N"), UserId = user.Id,
// ExpiresAt = DateTime.UtcNow.AddDays(7), IsUsed = false,
// IsRevoked = false
//         };
//         _context.RefreshTokens.Add(refreshToken);
//         await _context.SaveChangesAsync();
// return Ok(new
//         {
//             accessToken,
//             refreshToken = refreshToken.Token
//         });
// }
// }










// // using Microsoft.AspNetCore.Mvc;
// // using TmsApi.Domain.Entities;
// // using TmsApi.Application.Services;
// // using TmsApi.Application.Dtos;

// // [ApiController]
// // [Route("api/{version:apiVersion}/auth")]
// // public class AuthController : ControllerBase
// // {

// //     [HttpPost("login")]
// //     public IActionResult Login(
// //               [FromBody] LoginRequest request,
// //               [FromServices] IWebHostEnvironment env)
// //     {
// //         if (request.Username == "admin" && request.Password == "Password123!")
// //         {
// //             var dummyJwt = "header.payload.signature-demo-token";
// //             Response.Cookies.Append("tms_auth", dummyJwt, new CookieOptions
// //             {
// //                 HttpOnly = true,
// //                 Secure = !env.IsDevelopment(),
// //                 SameSite = SameSiteMode.Strict,
// //                 Expires = DateTimeOffset.UtcNow.AddHours(2)
// //             });
// //             return Ok(new UserProfileDto("System Admin", "Admin"));
// //         }
// //         return Unauthorized(new { detail = "Invalid username or password." });
// //     }
// //     [HttpGet("me")]
// //     public IActionResult GetCurrentUser()
// //     {
// //         // Inspect cookie attached automatically by the browser on cross-origin requests
// //         if (Request.Cookies.TryGetValue("tms_auth", out _))
// //         {
// //             return Ok(new UserProfileDto("System Admin",
// //             "Admin"));
// //         }
// //         return Unauthorized(new { detail = "Session expired or missing authentication cookie." });
// //     }
// // }



