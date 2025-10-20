using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hook this to your "Forgot password" UI. It validates the email, calls the auth service
/// to send a password reset, and displays friendly messages in a TMP text field.
/// - Optionally assign a component that implements IAuthService (as a MonoBehaviour) in the inspector.
/// - If none provided the component will create a FirebaseAuthService at runtime.
/// </summary>
public class ForgotPasswordUI : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField emailInput;

    [Header("Controls")]
    [SerializeField] private Button sendButton;

    [Header("Feedback")]
    [SerializeField] private TMP_Text statusText;

    [Tooltip("Optional: assign a MonoBehaviour that implements IAuthService (custom service). If left empty the script will use FirebaseAuthService by default.")]
    [SerializeField] private MonoBehaviour authServiceComponent;

    // not serialized: runtime service instance
    private IAuthService _authService;

    private void Awake()
    {
        if (sendButton != null) sendButton.onClick.AddListener(() => _ = OnSendClicked());

        // Try to get IAuthService if the user assigned a component
        if (authServiceComponent is IAuthService svc) _authService = svc;
    }

    private void OnDestroy()
    {
        if (sendButton != null) sendButton.onClick.RemoveListener(() => _ = OnSendClicked());
    }

    private async Task OnSendClicked()
    {
        if (statusText) statusText.text = string.Empty;

        var email = emailInput != null ? emailInput.text?.Trim() ?? string.Empty : string.Empty;

        if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
        {
            ShowError("อีเมล์ที่กรอกมาไม่ถูกต้อง");
            return;
        }

        if (sendButton) sendButton.interactable = false;
        ShowInfo("กำลังส่งคำแนะนำในการรีเซ็ตรหัสผ่าน...\nหากอีเมล์นี้ถูกลงทะเบียน คุณจะได้รับคำแนะนำในไม่ช้า");

        // Ensure we have an auth service
        if (_authService == null)
        {
            // Lazy create the default Firebase-based implementation
            try
            {
                await FirebaseReady.Ensure();
                _authService = new FirebaseAuthService(FirebaseAuth.DefaultInstance);
            }
            catch (System.Exception ex)
            {
                ShowError("Unable to initialize authentication service: " + ex.Message);
                if (sendButton) sendButton.interactable = true;
                return;
            }
        }

        var (ok, err) = await _authService.SendPasswordResetAsync(email);

        if (ok)
        {
            // Privacy-friendly success message (don't reveal whether email exists)
            ShowSuccess("ถ้าอีเมล์นี้ถูกลงทะเบียน เราได้ส่งคำแนะนำในการรีเซ็ตรหัสผ่าน โปรดตรวจสอบกล่องจดหมาย");
        }
        else
        {
            // Map common error substrings to friendly messages, fallback to returned err
            var friendly = MapErrorToMessage(err);
            ShowError(friendly);
        }

        if (sendButton) sendButton.interactable = true;
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        // simple RFC-like check (not perfect but good enough for UI validation)
        try
        {
            return Regex.IsMatch(email,
                @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
                RegexOptions.IgnoreCase, System.TimeSpan.FromMilliseconds(250));
        }
        catch
        {
            return false;
        }
    }

    private string MapErrorToMessage(string err)
    {
        if (string.IsNullOrEmpty(err)) return "Failed to send reset email. Please try again.";

        var lower = err.ToLowerInvariant();
        if (lower.Contains("invalid") || lower.Contains("invalidemail")) return "อีเมล์ที่กรอกมาไม่ถูกต้อง";
        if (lower.Contains("notfound") || lower.Contains("user-not-found") || lower.Contains("user not found"))
            return "หากอีเมล์นี้ถูกลงทะเบียน คุณจะได้รับคำแนะนำในการรีเซ็ตรหัสผ่านในไม่ช้า"; // privacy
        if (lower.Contains("network")) return "เกิดข้อผิดพลาดเกี่ยวกับเครือข่าย โปรดตรวจสอบการเชื่อมต่อและลองอีกครั้ง";
        if (lower.Contains("too many")) return "มีการพยายามมากเกินไป โปรดรอและลองอีกครั้งในภายหลัง";

        // fallback: show the message (may be raw). You can customize this mapping further.
        return err;
    }

    private void ShowError(string msg)
    {
        if (statusText != null)
        {
            statusText.color = Color.red;
            statusText.text = msg;
        }
        else Debug.LogWarning(msg);
    }

    private void ShowInfo(string msg)
    {
        if (statusText != null)
        {
            statusText.color = Color.black;
            statusText.text = msg;
        }
        else Debug.Log(msg);
    }

    private void ShowSuccess(string msg)
    {
        if (statusText != null)
        {
            statusText.color = Color.black;
            statusText.text = msg;
        }
        else Debug.Log(msg);
    }
}
