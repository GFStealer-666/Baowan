using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class AuthUIController : MonoBehaviour
{
    // registering side.
    public TMP_InputField nameInput, emailInput, phoneInput, passwordInput;
    public TMP_InputField ageInput, weightInput, heightInput, careerInput, glucoseInput;
    public TMP_Text statusText;

    IAuthService Auth => Services.Auth;
    IUserProfileRepository Repo => Services.Profiles;

    public void OnRegisterClicked() 
    {
        _ = RegisterFlowAsync();
    }
    private async Task RegisterFlowAsync()
    {
        var name  = nameInput.text.Trim();
        var email = emailInput.text.Trim();
        var pass  = passwordInput.text;
        var phone = phoneInput.text.Trim();

        SetStatus("Creating account...");

        // Create account (Firebase signs you in automatically)
        var (ok, user, err) = await AuthService.Instance.RegisterAndReturnUser(email, pass);
        if (!ok) { SetStatus(err); return; }

        // Hydrate user fields (email, uid) just in case
        try { await user.ReloadAsync(); } catch {}

        // NOW create the profile document
        try
        {
            Debug.Log($"[AuthUIController] Before EnsureProfile: DefaultAuthUid={Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser?.UserId} createdUserUid={user.UserId}");
            await UserProfileService.Instance.EnsureProfile(user, name, phone);
            Debug.Log("EnsureProfile DONE");
        }
        catch (Exception ex)
        {
            Debug.LogError("EnsureProfile failed: " + ex);
            SetStatus("Could not create profile.");
            return;
        }

        // (Optional) Claim display name atomically
        var claimed = await UserProfileService.TryClaimDisplayName(name);
        if (!claimed)
        {
            SetStatus("That name was just taken. Please choose another.");
            return;
        }

        SetStatus("Registered successfully!");
    }

    public async void OnSaveProfileClicked()
    {
        if (string.IsNullOrEmpty(Auth.CurrentUserId)) { SetStatus("Sign in first."); return; }
        var p = ParseProfileFromUI();
        await Repo.SaveAsync(Auth.CurrentUserId, p);
        SetStatus("Profile saved.");
    }

    UserProfile ParseProfileFromUI()
    {
        var p = new UserProfile {
            displayName = nameInput?.text.Trim(),
            phone = phoneInput?.text.Trim(),
            career = careerInput?.text.Trim()
        };
        if (int.TryParse(ageInput?.text, out int age)) p.age = Mathf.Clamp(age, 0, 120);
        if (double.TryParse(weightInput?.text, out var w)) p.weightKg = Math.Max(0, w);
        if (double.TryParse(heightInput?.text, out var h)) p.heightCm = Math.Max(0, h);
        if (double.TryParse(glucoseInput?.text, out var g)) p.bloodGlucoseMgDl = Math.Max(0, g);
        return p;
    }

    void SetStatus(string m) { if (statusText) statusText.text = m; Debug.Log(m); }
}
