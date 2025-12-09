using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using System.Threading.Tasks;

public class AuthStartup : MonoBehaviour
{
    [SerializeField] string appScene = "App";
    [SerializeField] string menuScene = "Mainmenu";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        // 1) Wait for Firebase to be ready (single global check)
        try
        {
            await FirebaseReady.Ensure();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AuthStartup] Firebase not available: {e}");
            SceneManager.LoadScene(menuScene);
            return;
        }

        // 2) Safe to use Firebase
        var auth = FirebaseAuth.DefaultInstance;

        // 3) Decide target scene
        var user = auth.CurrentUser;
        if (user != null)
        {
            try { await user.ReloadAsync(); } catch { }
            SceneManager.LoadScene(appScene);
        }
        else
        {
            SceneManager.LoadScene(menuScene);
        }
    }
}
