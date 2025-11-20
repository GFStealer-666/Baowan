using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SignOutButton : MonoBehaviour
{
    [Tooltip("Login/Mainmenu scene after signing out")]
    public string loginSceneName = "Mainmenu";

    [Tooltip("Optional Text to show status")]
    public Text statusText;

    public void OnSignOutPressed()
    {
        if (AuthService.Instance == null)
        {
            Debug.LogWarning("[SignOut] AuthService instance not found.");
            if (statusText != null) statusText.text = "Service unavailable.";
            return;
        }

        AuthService.Instance.SignOut();

        if (statusText != null)
            statusText.text = "Signed out";

        if (!string.IsNullOrEmpty(loginSceneName))
        {
            SceneManager.LoadScene(loginSceneName);
        }
        else
        {
            Debug.LogWarning("[SignOut] loginSceneName is empty — not loading any scene.");
        }
    }
}
