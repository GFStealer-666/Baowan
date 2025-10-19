using UnityEngine;

namespace BW.UI
{
    /// <summary>Concrete screen for a Canvas root.</summary>
    [DisallowMultipleComponent]
    public sealed class CanvasScreen : MonoBehaviour, IUIScreen
    {
    // New: use enum dropdown in inspector to avoid manual typing.
    [SerializeField] private BW.UI.ScreenId screenId = BW.UI.ScreenId.None;
    [SerializeField, HideInInspector] private string id = "ScreenId";   // legacy string id for compatibility
        [SerializeField] private bool startHidden = true;

        public string Id => string.IsNullOrEmpty(id) || id == "ScreenId" ? screenId.ToString() : id;
    // Expose the enum id under a different name to avoid conflict with the type name
    public ScreenId EnumId => screenId;

        public GameObject Root => gameObject;
        public bool IsVisible { get; private set; }

        private CanvasGroup _cg;

        private void Awake()
        {
            _cg = GetComponent<CanvasGroup>();
            if (startHidden) Hide(); else Show();
        }

        public void Show()
        {
            if (_cg) { _cg.alpha = 1f; _cg.interactable = true; _cg.blocksRaycasts = true; }
            gameObject.SetActive(true);
            IsVisible = true;
        }

        public void Hide()
        {
            if (_cg) { _cg.alpha = 0f; _cg.interactable = false; _cg.blocksRaycasts = false; }
            else gameObject.SetActive(false);
            IsVisible = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // If the legacy string hasn't been set, prefer enum value, otherwise keep manual override.
            if (string.IsNullOrWhiteSpace(id) || id == "ScreenId") id = screenId != BW.UI.ScreenId.None ? screenId.ToString() : name;
        }
#endif
    }
}
