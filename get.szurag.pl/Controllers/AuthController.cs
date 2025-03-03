using get.szurag.pl.Data;
using get.szurag.pl.Requests;
using get.szurag.pl.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace get.szurag.pl.Controllers;

[ApiController]
public class AuthController(ApplicationDbContext context) : ControllerBase
{
    [Route("api/auth")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var users = await context.Users.ToListAsync();
        
        foreach (var user in users)
        {
            var isValid = OtpService.ValidateOtp(user.SecretKey, request.OtpCode);
            if (isValid)
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                return Ok(new { message = "Login successful" });
            }
        }

        return Unauthorized(new { message = "Invalid OTP code" });
    }
}

