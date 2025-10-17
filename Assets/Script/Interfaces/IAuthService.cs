using System.Threading.Tasks;

public interface IAuthService
{
    Task<(bool ok, string err)> RegisterAsync(string email, string password);
    Task<(bool ok, string err)> LoginAsync(string email, string password);
    Task<(bool ok, string err)> SendPasswordResetAsync(string email);
    string CurrentUserId { get; }
    string CurrentEmail { get; }
    void SignOut();
}

public interface IUserProfileRepository
{
    Task EnsureProfileAsync(string uid, string email, string displayName, string phone);
    Task SaveAsync(string uid, UserProfile profile);
    Task<UserProfile> GetAsync(string uid);
}