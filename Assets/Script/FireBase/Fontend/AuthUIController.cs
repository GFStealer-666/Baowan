using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthUIController : MonoBehaviour
{
    // Registering side.
    [Header("Input Fields - Basic")]
    public TMP_InputField nameInput, emailInput, phoneInput, passwordInput;

    [Header("Input Fields - Extra Info")]
    public TMP_InputField ageInput, weightInput, heightInput, careerInput, glucoseInput;

    [Header("UI")]
    public TMP_Text statusText;
    public GameObject addInfoPanel, registerPanel;
    public string sceneToLoadOnSuccess = "App";

    // Services (from your service locator)
    // Services (from your service locator)
    IAuthService Auth => Services.Auth ?? AuthService.Instance;
    IUserProfileRepository Repo => Services.Profiles;


    private bool _ready;

    private async void Awake()
    {
        // Make sure Firebase + Services are ready before this UI is used
        try
        {
            await FirebaseReady.Ensure();
        }
        catch (Exception e)
        {
            Debug.LogError("[AuthUIController] Firebase / services not ready: " + e);
            SetStatus("ระบบยังไม่พร้อม กรุณาลองใหม่อีกครั้ง");
            enabled = false;
            return;
        }

        Debug.Log($"[AuthUIController] Ready. Auth={Auth != null}, Repo={Repo != null}, " +
                  $"AuthService.Instance={AuthService.Instance != null}, " +
                  $"UserProfileService.Instance={UserProfileService.Instance != null}");

        _ready = true;
    }

    // ---------- Register flow ----------

    public void OnRegisterClicked()
    {
        if (!_ready)
        {
            SetStatus("ระบบยังไม่พร้อม กรุณาลองใหม่อีกครั้ง");
            Debug.LogWarning("[AuthUIController] Register clicked before ready.");
            return;
        }

        _ = RegisterFlowAsync();
    }

    private async Task RegisterFlowAsync()
    {
        if (AuthService.Instance == null)
        {
            SetStatus("ระบบยังไม่พร้อม (AuthService)");
            Debug.LogError("[AuthUIController] AuthService.Instance is null");
            return;
        }
        if (UserProfileService.Instance == null)
        {
            SetStatus("ระบบยังไม่พร้อม (UserProfileService)");
            Debug.LogError("[AuthUIController] UserProfileService.Instance is null");
            return;
        }

        var name  = nameInput ? nameInput.text.Trim()   : string.Empty;
        var email = emailInput ? emailInput.text.Trim() : string.Empty;
        var pass  = passwordInput ? passwordInput.text  : string.Empty;
        var phone = phoneInput ? phoneInput.text.Trim() : string.Empty;

        SetStatus("กำลังลงทะเบียน...");

        // Create account (Firebase signs you in automatically)
        (bool ok, Firebase.Auth.FirebaseUser user, string err) result;
        try
        {
            result = await AuthService.Instance.RegisterAndReturnUser(email, pass);
        }
        catch (Exception ex)
        {
            Debug.LogError("[AuthUIController] RegisterAndReturnUser exception: " + ex);
            SetStatus("ไม่สามารถลงทะเบียนได้ กรุณาลองใหม่อีกครั้ง");
            return;
        }

        if (!result.ok)
        {
            SetStatus(result.err);
            return;
        }

        var user = result.user;

        // Hydrate user fields (email, uid) just in case
        try { await user.ReloadAsync(); } catch { }

        // NOW create the profile document
        try
        {
            Debug.Log($"[AuthUIController] Before EnsureProfile: DefaultAuthUid=" +
                      $"{Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser?.UserId} " +
                      $"createdUserUid={user.UserId}");

            await UserProfileService.Instance.EnsureProfile(user, name, phone);
            Debug.Log("[AuthUIController] EnsureProfile DONE");
        }
        catch (Exception ex)
        {
            Debug.LogError("[AuthUIController] EnsureProfile failed: " + ex);
            SetStatus("ไม่สามารถสร้างโปรไฟล์ได้");
            return;
        }

        // (Optional) Claim display name atomically
        bool claimed;
        try
        {
            claimed = await UserProfileService.TryClaimDisplayName(name);
        }
        catch (Exception ex)
        {
            Debug.LogError("[AuthUIController] TryClaimDisplayName failed: " + ex);
            SetStatus("ไม่สามารถตรวจสอบชื่อได้ กรุณาลองใหม่");
            return;
        }

        if (!claimed)
        {
            SetStatus("ชื่อนี้ถูกใช้ไปแล้ว กรุณาเลือกชื่ออื่น");
            return;
        }

        SetStatus("ลงทะเบียนสำเร็จ");

        if (registerPanel) registerPanel.SetActive(false);
        if (addInfoPanel) addInfoPanel.SetActive(true);
    }

    // ---------- Save extra profile info ----------

    public async void OnSaveProfileClicked()
    {
        if (!_ready)
        {
            SetStatus("ระบบยังไม่พร้อม กรุณาลองใหม่อีกครั้ง");
            Debug.LogWarning("[AuthUIController] Save profile clicked before ready.");
            return;
        }

        // Take local refs so we don't call the property twice
        var auth = Auth;
        var repo = Repo;

        if (auth == null)
        {
            Debug.LogError("[AuthUIController] Auth service is NULL in OnSaveProfileClicked");
            SetStatus("ระบบยังไม่พร้อม (Auth)");
            return;
        }

        if (repo == null)
        {
            Debug.LogError("[AuthUIController] Profile repository is NULL in OnSaveProfileClicked");
            SetStatus("ระบบยังไม่พร้อม (Profile)");
            return;
        }

        var uid = auth.CurrentUserId;
        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogError("[AuthUIController] CurrentUserId is null/empty in OnSaveProfileClicked");
            SetStatus("กรุณาเข้าสู่ระบบก่อน");
            return;
        }

        var p = ParseProfileFromUI();

        try
        {
            await repo.SaveAsync(uid, p);
        }
        catch (Exception ex)
        {
            Debug.LogError("[AuthUIController] Repo.SaveAsync failed: " + ex);
            SetStatus("ไม่สามารถบันทึกโปรไฟล์ได้");
            return;
        }

        SetStatus("โปรไฟล์ถูกบันทึกแล้ว");

        if (addInfoPanel != null)
            addInfoPanel.SetActive(false);
        else
            Debug.LogWarning("[AuthUIController] addInfoPanel is not assigned in inspector");

        if (!string.IsNullOrEmpty(sceneToLoadOnSuccess))
            SceneManager.LoadScene(sceneToLoadOnSuccess);
        else
            Debug.LogWarning("[AuthUIController] sceneToLoadOnSuccess is empty");
    }



    // ---------- Helpers ----------

    UserProfile ParseProfileFromUI()
    {
        string Clean(string s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        var p = new UserProfile
        {
            displayName = Clean(nameInput ? nameInput.text : null),
            phone       = Clean(phoneInput ? phoneInput.text : null),
            career      = Clean(careerInput ? careerInput.text : null)
        };

        if (int.TryParse(ageInput ? ageInput.text : null, out var age))
            p.age = Mathf.Clamp(age, 0, 120);

        if (double.TryParse(weightInput ? weightInput.text : null, out var w))
            p.weightKg = Math.Max(0, w);

        if (double.TryParse(heightInput ? heightInput.text : null, out var h))
            p.heightCm = Math.Max(0, h);

        if (double.TryParse(glucoseInput ? glucoseInput.text : null, out var g))
            p.bloodGlucoseMgDl = Math.Max(0, g);

        return p;
    }

    void SetStatus(string m)
    {
        if (statusText) statusText.text = m;
        Debug.Log("[AuthUI] " + m);
    }
}
