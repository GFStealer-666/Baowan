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
    public Text statusText;
    public string sceneToLoadOnSuccess;

    Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    public async void OnLoginPressed()
    {
        if (AuthService.Instance == null)
        {
            SetStatus("AuthService not initialized.");
            Debug.LogWarning("AuthService.Instance is null when trying to login.");
            return;
        }

        string email = GetEmail()?.Trim();
        string password = GetPassword();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            SetStatus("Please enter email and password.");
            return;
        }

        // disable button while signing in
        SetInteractable(false);
        SetStatus("Signing in...");

        try
        {
            try { email = EmailFormatter.CleanEmail(email); } catch { }

            var result = await AuthService.Instance.Login(email, password);
            var ok = result.ok;
            var err = result.err;

            if (ok)
            {
                SetStatus("Signed in");
                if (!string.IsNullOrEmpty(sceneToLoadOnSuccess))
                    SceneManager.LoadScene(sceneToLoadOnSuccess);
            }
            else
            {
                SetStatus("Sign in failed: " + (err ?? "Unknown error"));
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            SetStatus("Sign in error: " + ex.Message);
        }
        finally
        {
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
        Debug.Log("[LoginButton] " + s);
    }

    void SetInteractable(bool v)
    {
        if (_button != null) _button.interactable = v;
    }
}
