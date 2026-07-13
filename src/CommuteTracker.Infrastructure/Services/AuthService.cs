using System.Threading.Tasks;
using CommuteTracker.Core.Entities;
using CommuteTracker.Core.Helpers;
using CommuteTracker.Core.Services.Interfaces;
namespace CommuteTracker.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly CommuteTrackerDbContext _db;
        public AuthService(CommuteTrackerDbContext db)
        {
            _db = db;
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

        public Task<string?> LoginUserAsync(string email, string password)
        {
            // TODO: add actual login logic (e.g. verify credentials, return token)
            return Task.FromResult<string?>("User logged in");
        }
    }
}
