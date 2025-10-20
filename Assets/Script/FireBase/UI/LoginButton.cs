using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Attach to a UI Button (or any GameObject). Hook the Button.OnClick to OnLoginPressed().
/// Supports UnityEngine.UI.InputField and TextMeshPro TMP_InputField for email/password.
/// Calls AuthService.Instance.Login(email,password) and updates optional status Text.
/// </summary>
public class LoginButton : MonoBehaviour
{
    [Header("Inputs")]

    public TMP_InputField emailTMP;
    public TMP_InputField passwordTMP;

    [Header("Optional UI")]
    public TMP_Text statusText;
    public string sceneToLoadOnSuccess;

    [Header("Success handling")]
    [Tooltip("Optional panel to show after successful login (e.g. add info). If set, this panel will be activated on success instead of loading a scene.")]
    public GameObject addInfoPanel;
    [Tooltip("Optional root of the login UI to hide when success panel is shown.")]
    public GameObject loginRoot;
    [Tooltip("If true and Add Info panel is shown, the loginRoot will be hidden on success.")]
    public bool hideLoginOnSuccess = true;

    Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    public async void OnLoginPressed()
    {
        if (AuthService.Instance == null)
        {
            SetStatus("ระบบยืนยันตัวตนยังไม่ถูกตั้งค่า");
            Debug.LogWarning("AuthService.Instance is null when trying to login.");
            return;
        }

        string email = GetEmail()?.Trim();
        string password = GetPassword();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            SetStatus("กรุณากรอกอีเมลและรหัสผ่าน");
            return;
        }

            // disable button while signing in
            SetInteractable(false);
            SetStatus("กำลังเข้าสู่ระบบ...");

        try
        {
            try { email = EmailFormatter.CleanEmail(email); } catch { }

            var result = await AuthService.Instance.Login(email, password);
            var ok = result.ok;
            var err = result.err;

            if (ok)
            {
                SetStatus("เข้าสู่ระบบสำเร็จ");

                // Prefer showing Add Info panel if assigned
                if (addInfoPanel != null)
                {
                    addInfoPanel.SetActive(true);
                    if (hideLoginOnSuccess && loginRoot != null) loginRoot.SetActive(false);
                    // keep button disabled when we've moved to the add-info flow
                    return;
                }

                if (!string.IsNullOrEmpty(sceneToLoadOnSuccess))
                {
                    SceneManager.LoadScene(sceneToLoadOnSuccess);
                    return;
                }
            }
            else
            {
                SetStatus("เข้าสู่ระบบล้มเหลว: " + (err ?? "เกิดข้อผิดพลาดไม่ทราบสาเหตุ"));
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            SetStatus("ข้อผิดพลาดการเข้าสู่ระบบ: " + ex.Message);
        }
        finally
        {
            // If addInfoPanel was shown we returned and won't reach here; re-enable otherwise
            SetInteractable(true);
        }
    }

    string GetEmail()
    {
        if (emailTMP != null && !string.IsNullOrEmpty(emailTMP.text)) return emailTMP.text;
        return null;
    }

    string GetPassword()
    {
        if (passwordTMP != null && !string.IsNullOrEmpty(passwordTMP.text)) return passwordTMP.text;
        return null;
    }

    void SetStatus(string s)
    {
        if (statusText != null) statusText.text = s;
        else Debug.Log("[LoginButton] " + s);
    }

    void SetInteractable(bool v)
    {
        if (_button != null) _button.interactable = v;
    }
}
