using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

public class AuthService : MonoBehaviour, IAuthService
{
    public static AuthService Instance { get; private set; }
    public FirebaseAuth Auth { get; private set; }
    public FirebaseUser CurrentUser => Auth?.CurrentUser;
    public string CurrentUserId => Auth?.CurrentUser?.UserId;
    public string CurrentEmail  => Auth?.CurrentUser?.Email;

    public static event Action OnSignOut;
     async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        try
        {
            await FirebaseReady.Ensure();
        }
        catch (Exception e)
        {
            Debug.LogError("[AuthService] Firebase init failed in Awake: " + e);
            return;
        }

        Auth = FirebaseAuth.DefaultInstance;
        Debug.Log("[AuthService] FirebaseAuth.DefaultInstance initialised");
    }

    async void Start()
    {
        await Services.InitAsync();

        // Optional but harmless to reassign:
        Auth = FirebaseAuth.DefaultInstance;

        Auth.IdTokenChanged += (s, e) => _ = HandleIdTokenChangedAsync();
        Auth.StateChanged   += OnAuthStateChanged;

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

        public async Task<(bool ok, string err)> RegisterAsync(string email, string password)
    {
        try
        {
            email = EmailFormatter.CleanEmail(email);
            if (!EmailFormatter.LooksLikeEmail(email))
                return (false, "Invalid email format");

            var result = await FirebaseAuth.DefaultInstance
                .CreateUserWithEmailAndPasswordAsync(email, password);
            var user = result.User;
            // optional: update profile here
            return (true, null);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return (false, ParseFirebaseError(ex));
        }
    }

    public async Task<(bool ok, string err)> LoginAsync(string email, string password)
    {
        try
        {
            await FirebaseReady.Ensure();

            email    = (email ?? "").Trim().Replace("\r", "").Replace("\n", "");
            password = password ?? "";

            var result = await FirebaseAuth.DefaultInstance
                .SignInWithEmailAndPasswordAsync(email, password);

            // optional: await result.User.ReloadAsync();
            return (true, null);
        }
        catch (FirebaseException fex)
        {
            var code = (AuthError)fex.ErrorCode;
            Debug.LogError($"[Auth] Login failed: {code} ({fex.ErrorCode}) {fex.Message}");

            string msg = code switch
            {
                AuthError.InvalidEmail         => "รูปแบบอีเมลไม่ถูกต้อง",
                AuthError.MissingEmail         => "กรุณากรอกอีเมลของคุณ",
                AuthError.MissingPassword      => "กรุณากรอกรหัสผ่านของคุณ",
                AuthError.WrongPassword        => "รหัสผ่านไม่ถูกต้อง",
                AuthError.UserNotFound         => "ไม่พบบัญชีที่มีอีเมลนี้",
                AuthError.NetworkRequestFailed => "เกิดข้อผิดพลาดในการเชื่อมต่อ โปรดลองอีกครั้ง",
                AuthError.OperationNotAllowed  => "การเข้าสู่ระบบด้วยอีเมล/รหัสผ่านถูกปิดใช้งานใน Firebase",
                _ => "การเข้าสู่ระบบล้มเหลว โปรดลองอีกครั้ง"
            };
            return (false, msg);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return (false, "Unexpected error. Please try again.");
        }
    }

    public async Task<(bool ok, string err)> SendPasswordResetAsync(string email)
    {
        try
        {
            await Auth.SendPasswordResetEmailAsync(email);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ParseFirebaseError(ex));
        }
    }

    public void SignOut()
    {
        if (Auth == null)
        {
            Debug.LogError("[AuthService] SignOut called but Auth is null");
            return;
        }

        Auth.SignOut();
        OnSignOut?.Invoke();

        try
        {
            FirebaseAuth.DefaultInstance.SignOut();

            PlayerPrefs.DeleteKey("user_email");
            PlayerPrefs.DeleteKey("user_name");
            PlayerPrefs.Save();

            Debug.Log("[AuthService] SignOut success.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthService] SignOut error: {ex}");
        }
    }
    public async System.Threading.Tasks.Task HandleIdTokenChangedAsync()
    {
        var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        var user = auth.CurrentUser;

        // Signed out? Just exit cleanly.
        if (user == null || string.IsNullOrWhiteSpace(user.UserId))
        {
            UnityEngine.Debug.Log("[Auth] IdTokenChanged: user is null (signed out). Skip EnsureProfile.");
            return;
        }

        // Optionally ensure Firebase/Firestore ready here if you have a helper
        await FirebaseReady.Ensure();

        try
        {
            await UserProfileService.Instance.EnsureProfile(user, user.DisplayName, user.PhoneNumber);
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"[Auth] EnsureProfile failed: {ex}");
        }
    }


    public static async Task<FirebaseUser> WaitForUserWithUid(int timeoutMs = 8000)
    {
        await FirebaseReady.Ensure();
        var auth = FirebaseAuth.DefaultInstance;

        var start = Time.realtimeSinceStartup;
        while (auth.CurrentUser == null || string.IsNullOrEmpty(auth.CurrentUser.UserId))
        {
            // small delay to avoid tight spinning; allow main thread to process callbacks
            await Task.Delay(50);
            if ((Time.realtimeSinceStartup - start) * 1000 > timeoutMs)
                throw new TimeoutException("Timed out waiting for signed-in user with UID.");
        }
        return auth.CurrentUser;
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

    string ParseFirebaseError(Exception ex)
    {
        Debug.LogError(ex);
        if (ex is AggregateException ag && ag.InnerException != null) ex = ag.InnerException;
        return ex.Message;
    }


}
