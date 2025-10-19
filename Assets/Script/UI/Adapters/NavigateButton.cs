using UnityEngine;
using UnityEngine.UI;

namespace BW.UI
{
    /// <summary>Attach to a Button to navigate to a target screen.</summary>
    [RequireComponent(typeof(Button))]
    public sealed class NavigateButton : MonoBehaviour
    {
        [SerializeField] private UINavigationService navigation;
        // New: pick the target screen from enum in the inspector
        [SerializeField] private BW.UI.ScreenId targetScreen = BW.UI.ScreenId.None;
        // Legacy: keep string for existing serialized scenes/prefabs
        [SerializeField, HideInInspector] private string targetScreenId;
        [SerializeField] private bool pushToHistory = true;

        private void Awake()
        {
            if(navigation == null)
            {
                navigation = FindAnyObjectByType<UINavigationService>();
            }
            var btn = GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                if (navigation == null)
                {
                    Debug.LogWarning("[NavigateButton] Navigation not assigned.");
                    return;
                }
                if (navigation == null) return;
                // Prefer enum when set, otherwise use legacy string id
                if (targetScreen != BW.UI.ScreenId.None)
                {
                    // Use enum overload if available
                    navigation.GoTo(targetScreen, pushToHistory);
                }
                else
                {
                    navigation.GoTo(targetScreenId, pushToHistory);
                }
            });
        }
    }
}
