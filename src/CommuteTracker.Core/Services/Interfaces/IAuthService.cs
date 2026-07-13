namespace CommuteTracker.Core.Services.Interfaces;
public interface IAuthService
    {
       Task<string> RegisterUserAsync(string name, string email, string password);
         Task<string?> LoginUserAsync(string email, string password);
    }
