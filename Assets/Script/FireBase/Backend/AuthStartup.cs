using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;

public class AuthStartup : MonoBehaviour
{
    [SerializeField] string appScene = "App";
    [SerializeField] string menuScene = "Mainmenu";

    // Keep Awake minimal; do NOT call Firebase here.
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        // 1) Wait for Firebase to be ready
        var dep = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dep != DependencyStatus.Available)
        {
            Debug.LogError($"Firebase not available: {dep}");
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
