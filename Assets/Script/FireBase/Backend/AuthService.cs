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
        Auth.IdTokenChanged += (s, e) => _ = HandleIdTokenChangedAsync();

        async Task HandleIdTokenChangedAsync()
        {
            try
            {
                // First try the current user immediately
                var u = FirebaseAuth.DefaultInstance.CurrentUser;
                if (u == null || string.IsNullOrEmpty(u.UserId))
                {
                    // Wait a bit for the system to populate the user; this may timeout sometimes
                    u = await WaitForUserWithUid(12000);
                }

                // If still null, bail quietly
                if (u == null) return;

                await u.TokenAsync(true);               // fresh token
                await UserProfileService.Instance.EnsureProfile(u);
            }
            catch (TimeoutException tex)
            {
                // Not fatal; log a warning and continue. We may pick this up later via other flows.
                Debug.LogWarning("[Auth] EnsureProfile after IdTokenChanged timed out waiting for user: " + tex.Message);
            }
            catch (Exception ex)
            {
                Debug.LogError("[Auth] EnsureProfile after IdTokenChanged failed: " + ex);
            }
        }
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
            // small delay to avoid tight spinning; allow main thread to process callbacks
            await Task.Delay(50);
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
            email = (email ?? "").Trim().Replace("\r", "").Replace("\n", "");
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
                AuthError.InvalidEmail => "รูปแบบอีเมลไม่ถูกต้อง",
                AuthError.MissingEmail => "กรุณากรอกอีเมลของคุณ",
                AuthError.MissingPassword => "กรุณากรอกรหัสผ่านของคุณ",
                AuthError.WrongPassword => "รหัสผ่านไม่ถูกต้อง",
                AuthError.UserNotFound => "ไม่พบบัญชีที่มีอีเมลนี้",
                AuthError.NetworkRequestFailed => "เกิดข้อผิดพลาดในการเชื่อมต่อ โปรดลองอีกครั้ง",
                AuthError.OperationNotAllowed => "การเข้าสู่ระบบด้วยอีเมล/รหัสผ่านถูกปิดใช้งานใน Firebase",
                _ => "การเข้าสู่ระบบล้มเหลว โปรดลองอีกครั้ง"
            };
            return (false, msg);
        }
    }
    
    private async System.Threading.Tasks.Task HandleIdTokenChangedAsync()
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
