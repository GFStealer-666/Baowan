using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Small helper to wire a UI Button to the AuthService.SignOut method.
public class SignOutButton : MonoBehaviour
{
    [Tooltip("Optional Text to show auth status after sign out")]
    public Text statusText;

    // Called by the UI Button OnClick
    public void OnSignOutPressed()
    {
        if (AuthService.Instance == null)
        {
            Debug.LogWarning("AuthService instance not found.");
            if (statusText != null) statusText.text = "AuthService not initialized.";
            return;
        }

        AuthService.Instance.SignOut();
        Debug.Log("[SignOutButton] SignOut called.");
        if (statusText != null) statusText.text = "Signed out";
    }
}
