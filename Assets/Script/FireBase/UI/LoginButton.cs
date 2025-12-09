using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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

    // Use the same pattern as AuthUIController: service locator with fallback
    IAuthService Auth => Services.Auth ?? AuthService.Instance;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    public async void OnLoginPressed()
    {
        var auth = Auth;
        if (auth == null)
        {
            SetStatus("ระบบยืนยันตัวตนยังไม่พร้อม (Auth)");
            Debug.LogWarning("[LoginButton] Auth service is null when trying to login.");
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

            // Call through IAuthService – implemented by AuthService
            var (ok, err) = await auth.LoginAsync(email, password);

            if (ok)
            {
                SetStatus("เข้าสู่ระบบสำเร็จ");

                // If you want the Add Info panel flow after login, uncomment this block
                if (addInfoPanel != null)
                {
                    addInfoPanel.SetActive(true);
                    if (hideLoginOnSuccess && loginRoot != null)
                        loginRoot.SetActive(false);

                    // We’re moving to another UI flow; keep button disabled
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
            Debug.LogError("[LoginButton] Login exception: " + ex);
            SetStatus("ข้อผิดพลาดการเข้าสู่ระบบ: " + ex.Message);
        }
        finally
        {
            // If we returned earlier for Add Info / scene load, we never reach here
            SetInteractable(true);
        }
    }

    string GetEmail()
    {
        if (emailTMP != null && !string.IsNullOrEmpty(emailTMP.text))
            return emailTMP.text;
        return null;
    }

    string GetPassword()
    {
        if (passwordTMP != null && !string.IsNullOrEmpty(passwordTMP.text))
            return passwordTMP.text;
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
