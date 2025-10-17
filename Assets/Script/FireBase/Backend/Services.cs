using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;

// FireBaseInitializer (backend)
public static class Services
{
    public static IAuthService Auth { get; private set; }
    public static IUserProfileRepository Profiles { get; private set; }

    public static async Task InitAsync()
    {
        var dep = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dep != DependencyStatus.Available) throw new Exception(dep.ToString());

        Auth = new FirebaseAuthService(FirebaseAuth.DefaultInstance);
        Profiles = new FirestoreUserProfileRepository(FirebaseFirestore.DefaultInstance);
    }
}