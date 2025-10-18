using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

public class AuthService : MonoBehaviour
{
    public static AuthService Instance { get; private set; }
    public FirebaseAuth Auth { get; private set; }
    public FirebaseUser CurrentUser => Auth?.CurrentUser;

    public static event Action OnSignOut;
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        await Services.InitAsync();
        Auth = FirebaseAuth.DefaultInstance;
        Auth.IdTokenChanged += async (s, e) =>
        {
            try
            {
                var u = await WaitForUserWithUid();     // ← guarantees non-empty UID
                await u.TokenAsync(true);               // fresh token
                await UserProfileService.Instance.EnsureProfile(u);
            }
            catch (Exception ex)
            {
                Debug.LogError("[Auth] EnsureProfile after IdTokenChanged failed: " + ex);
            }
        };
        Auth.StateChanged += OnAuthStateChanged;
        OnAuthStateChanged(this, null);
    }

    void OnDestroy()
    {
        if (Auth != null) Auth.StateChanged -= OnAuthStateChanged;
    }

    void OnAuthStateChanged(object sender, EventArgs e)
    {
        if (Auth.CurrentUser != null)
            Debug.Log($"[Auth] Signed in: {Auth.CurrentUser.Email}");
        else
            Debug.Log("[Auth] Signed out");
    }

    public async Task<(bool ok, string err)> Register(string email, string password, string displayName = null)
    {
        try
        {
            email = EmailFormatter.CleanEmail(email);
            if (!EmailFormatter.LooksLikeEmail(email)) return (false, "Invalid email format");

            var result = await FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password);
            var user = result.User;
            // ... update profile, reload, etc.
            return (true, null);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return (false, ParseFirebaseError(ex));
        }
    }

    public async Task<(bool ok, FirebaseUser user, string err)> RegisterAndReturnUser(string email, string password)
    {
        try
        {
            await FirebaseReady.Ensure();
            var result = await FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password);
            var user = result.User;

            // Wait for the Auth system to report the newly-created user as the current user
            var start = Time.realtimeSinceStartup;
            while (FirebaseAuth.DefaultInstance.CurrentUser?.UserId != user.UserId)
            {
                await System.Threading.Tasks.Task.Yield();
                if ((Time.realtimeSinceStartup - start) * 1000f > 5000f) break;
            }

            // Force refresh token so Firestore receives credentials
            try { await user.TokenAsync(true); } catch (Exception ex) { Debug.LogWarning("Token refresh after register failed: " + ex); }

            return (true, user, null);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return (false, null, ParseFirebaseError(ex));
        }
    }
    static async Task<FirebaseUser> WaitForUserWithUid(int timeoutMs = 8000)
    {
        await FirebaseReady.Ensure();
        var auth = FirebaseAuth.DefaultInstance;

        var start = Time.realtimeSinceStartup;
        while (auth.CurrentUser == null || string.IsNullOrEmpty(auth.CurrentUser.UserId))
        {
            await System.Threading.Tasks.Task.Yield();
            if ((Time.realtimeSinceStartup - start) * 1000 > timeoutMs)
                throw new TimeoutException("Timed out waiting for signed-in user with UID.");
        }
        return auth.CurrentUser;
    }
    public async Task<(bool ok, string err)> Login(string email, string password)
    {
        try
        {
            await FirebaseReady.Ensure();
            email = (email ?? "").Trim().Replace("\r","").Replace("\n","");
            password = password ?? "";

            var result = await FirebaseAuth.DefaultInstance
                .SignInWithEmailAndPasswordAsync(email, password);
            return (true, null);
        }
        catch (FirebaseException fex)
        {
            var code = (AuthError)fex.ErrorCode;
            Debug.LogError($"[Auth] Login failed: {code} ({fex.ErrorCode}) {fex.Message}");

            string msg = code switch
            {
                AuthError.InvalidEmail          => "Email format is invalid.",
                AuthError.MissingEmail          => "Please enter your email.",
                AuthError.MissingPassword       => "Please enter your password.",
                AuthError.WrongPassword         => "Wrong password.",
                AuthError.UserNotFound          => "No account found with this email.",
                AuthError.NetworkRequestFailed  => "Network error. Try another network.",
                AuthError.OperationNotAllowed   => "Email/password sign-in is disabled in Firebase.",
                _ => "Sign-in failed. Please try again."
            };
            return (false, msg);
        }
    }

    public async Task<(bool ok, string err)> SendPasswordReset(string email)
    {
        try
        {
            await Auth.SendPasswordResetEmailAsync(email);
            return (true, null);
        }
        catch (Exception ex) { return (false, ParseFirebaseError(ex)); }
    }

    public void SignOut()
    {
        Auth.SignOut();
        OnSignOut?.Invoke();
    }

    string ParseFirebaseError(Exception ex)
    {
        Debug.LogError(ex);
        if (ex is AggregateException ag && ag.InnerException != null) ex = ag.InnerException;
        return ex.Message;
    }
}
