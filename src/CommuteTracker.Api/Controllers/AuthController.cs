using Microsoft.AspNetCore.Mvc;
using System.Text;
using CommuteTracker.Api.DTOs;
using CommuteTracker.Core.Services.Interfaces;
using CommuteTracker.Infrastructure;

namespace CommuteTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    
    private readonly IAuthService _authService;

    public AuthController( IAuthService authService)
   {
    _authService = authService;
   }
   [HttpPost("register")]
public async Task<IActionResult> Register(RegisterRequest request)
{
    var userId = await _authService.RegisterUserAsync(request.Name, request.Email, request.Password);

    return Ok(new
    {
        Message = "User registered successfully",
        UserId = userId
    });
}
    [HttpPost("login")]
public IActionResult Login(LoginRequest request)
{
    var token = _authService.LoginUserAsync(request.Email, request.Password).Result;
    if (token == null)
    {
        return Unauthorized("Invalid email or password.");
    }
    return Ok(new
    {
       token
    });
}
}