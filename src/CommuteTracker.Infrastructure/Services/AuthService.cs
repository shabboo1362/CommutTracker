using System.Threading.Tasks;
using CommuteTracker.Core.Entities;
using CommuteTracker.Core.Helpers;
using CommuteTracker.Core.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
namespace CommuteTracker.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly CommuteTrackerDbContext _db;
        private readonly IConfiguration _config;
        public AuthService(CommuteTrackerDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<string> RegisterUserAsync(string name, string email, string password)
        {
            var existingUser = _db.Users
            .FirstOrDefault(u => u.Email == email);

            if (existingUser != null)
               {
                  throw new Exception("Email already exists.");
            }

            var user = new User
               {
                  Id = Guid.NewGuid(),
                  Name = name,
                  Email = email,
                  PasswordHash = PasswordHasher.Hash(password)
            };

           _db.Users.Add(user);

           await _db.SaveChangesAsync();
           return user.Id.ToString(); // Return the user ID or any other relevant information
        }

        public async Task<string?> LoginUserAsync(string email, string password)
        {
            // TODO: add actual login logic (e.g. verify credentials, return token)
            var user = _db.Users
                 .FirstOrDefault(u => u.Email == email);

            if (user == null)
              {
                 return null; 
              }

            var validPassword =
            PasswordHasher.Verify(
            password,
            user.PasswordHash);

            if (!validPassword)
            {
                return null;
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
