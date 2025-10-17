using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    public static SceneSwitcher Instance;
    [SerializeField] private string appScene;
    [SerializeField] private string mainmenuScene;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    private void OnEnable()
    {
        AuthService.OnSignOut += LoadMainmenu;
    }

    private void OnDisable()
    {
        AuthService.OnSignOut -= LoadMainmenu;
    }

    public void LoadingScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void LoadMainmenu()
    {
        SceneManager.LoadScene(mainmenuScene);
    }
    public void LoadApp()
    {
        SceneManager.LoadScene(appScene);
    }
}
