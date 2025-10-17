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

        // Try silent login (if persisted)
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

            // Optional profile update here...
            // await user.UpdateUserProfileAsync(new UserProfile { DisplayName = ... });

            return (true, user, null);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return (false, null, ParseFirebaseError(ex));
        }
    }

    public async Task<(bool ok, string err)> Login(string email, string password)
    {
        try
        {
            await Auth.SignInWithEmailAndPasswordAsync(email, password);
            return (true, null);
        }
        catch (Exception ex) { return (false, ParseFirebaseError(ex)); }
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
