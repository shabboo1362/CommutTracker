using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CommuteTracker.Api.DTOs;
using CommuteTracker.Core.Entities;
using CommuteTracker.Core.Helpers;
using CommuteTracker.Infrastructure;



namespace CommuteTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly CommuteTrackerDbContext _db;
    private readonly IConfiguration _config;
    public AuthController(CommuteTrackerDbContext db, IConfiguration config)
   {
    _config = config;
    _db = db;
   }
   [HttpPost("register")]
public async Task<IActionResult> Register(RegisterRequest request)
{
    var existingUser = _db.Users
        .FirstOrDefault(u => u.Email == request.Email);

    if (existingUser != null)
    {
        return BadRequest("Email already exists.");
    }

    var user = new User
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Email = request.Email,
        PasswordHash = PasswordHasher.Hash(request.Password)
    };

    _db.Users.Add(user);

    await _db.SaveChangesAsync();

    return Ok(new
    {
        Message = "User registered successfully"
    });
}
    [HttpPost("login")]
public IActionResult Login(LoginRequest request)
{
    var user = _db.Users
        .FirstOrDefault(u => u.Email == request.Email);

    if (user == null)
    {
        return Unauthorized("Invalid email or password.");
    }

    var validPassword =
        PasswordHasher.Verify(
            request.Password,
            user.PasswordHash);

    if (!validPassword)
    {
        return Unauthorized("Invalid email or password.");
    }

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Email, user.Email)
    };

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

    var creds = new SigningCredentials(
        key,
        SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: creds);

    return Ok(new
    {
        Token = new JwtSecurityTokenHandler()
            .WriteToken(token)
    });
}
}