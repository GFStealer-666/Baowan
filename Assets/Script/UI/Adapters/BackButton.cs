using UnityEngine;
using UnityEngine.UI;

namespace BW.UI
{
    /// <summary>Attach to a Button to go back in history.</summary>
    [RequireComponent(typeof(Button))]
    public sealed class BackButton : MonoBehaviour
    {
        [SerializeField] private UINavigationService navigation;

        private void Awake()
        {
            var btn = GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                if (navigation == null)
                {
                    Debug.LogWarning("[BackButton] Navigation not assigned.");
                    return;
                }
                navigation.Back();
            });
        }
    }
}
