using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;

public static class Services
{
    public static IAuthService Auth { get; private set; }
    public static IUserProfileRepository Profiles { get; private set; }

    public static async Task InitAsync()
    {
        // 1) Make sure Firebase is ready (only via FirebaseReady)
        await FirebaseReady.Ensure();

        // 2) Wire up service locator once
        if (Auth == null)
        {
            // AuthService is the single implementation of IAuthService
            Auth = AuthService.Instance;
        }

        if (Profiles == null)
        {
            Profiles = new FirestoreUserProfileRepository(
                FirebaseFirestore.DefaultInstance
            );
        }
    }
}
