using System;
using System.Collections.Generic;
using UnityEngine;

namespace BW.UI
{
    /// <summary>Navigation orchestrator (SRP: switching + history only).</summary>
    public sealed class UINavigationService : MonoBehaviour, INavigationService
    {
        [Header("Dependencies")]
        [SerializeField] private UIScreenRegistry registry;

    [Header("Startup")]
    // Optional: pick screens from enum dropdown in inspector to avoid typos.
    [SerializeField] private BW.UI.ScreenId startScreen = BW.UI.ScreenId.Home;
    [SerializeField] private BW.UI.ScreenId fallbackScreen = BW.UI.ScreenId.Home;
    // Legacy string fields (kept for serialization compatibility with existing scenes). Hidden in inspector.
    [SerializeField, HideInInspector] private string startScreenId = "Home";
    [SerializeField, HideInInspector] private string fallbackScreenId = "Home";

        private readonly Stack<string> _history = new();
        private IUIScreen _current;

        public string CurrentId => _current?.Id;
        public ScreenId Current => _current != null ? _current.EnumId : ScreenId.None;
        public event Action<string> OnScreenChanged;

        private string GetFallbackId()
        {
            return fallbackScreen != BW.UI.ScreenId.None ? fallbackScreen.ToString() : fallbackScreenId;
        }

        private void Start()
        {
            if (registry == null)
            {
                Debug.LogError("[UINavigationService] Registry is not assigned.");
                return;
            }
            // Prefer enum value if set, otherwise fall back to legacy string.
            var start = startScreen != BW.UI.ScreenId.None ? startScreen.ToString() : startScreenId;
            ResetTo(start);
        }

        // String-based API (existing callers keep working)
        public void ResetTo(string id)
        {
            _history.Clear();
            _ = TryOpenByString(id, push:false, allowFallback:true);
        }

        // Enum-based overload for type-safety and inspector-friendly usage
        public void ResetTo(BW.UI.ScreenId id)
        {
            _history.Clear();
            InternalOpen(id, push:false, allowFallback:true);
        }

        public bool GoTo(string id, bool pushToHistory = true)
        {
            return TryOpenByString(id, pushToHistory, allowFallback:true);
        }

        public bool GoTo(BW.UI.ScreenId id, bool pushToHistory = true) => InternalOpen(id, pushToHistory, allowFallback:true);

        public bool Back()
        {
            if (_history.Count == 0)
            {
                // nothing to go back to → go to fallback (prefer enum if set)
                return TryOpenByString(GetFallbackId(), push:false, allowFallback:false);
            }

            var prevId = _history.Pop();
            return TryOpenByString(prevId, push:false, allowFallback:false);
        }

        // Try to open by a string id: parse to enum first then try enum lookup; if parsing fails, use string lookup
        private bool TryOpenByString(string id, bool push, bool allowFallback)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Debug.LogWarning("[UINavigationService] Empty screen id.");
                return false;
            }

            // Try parse to enum and open
            if (System.Enum.TryParse<ScreenId>(id, true, out var sid) && sid != ScreenId.None)
            {
                return InternalOpen(sid, push, allowFallback);
            }

            // fallback to string-based registry lookup
            if (!registry.TryGet(id, out var next))
            {
                var fb = GetFallbackId();
                if (allowFallback && !string.IsNullOrEmpty(fb) && registry.TryGet(fb, out next))
                {
                    Debug.LogWarning($"[UINavigationService] Screen '{id}' not found. Using fallback '{fb}'.");
                }
                else
                {
                    Debug.LogWarning($"[UINavigationService] Screen '{id}' not found.");
                    return false;
                }
            }

            if (_current != null)
            {
                if (push) _history.Push(_current.Id);
                _current.Hide();
            }

            _current = next;
            _current.Show();
            OnScreenChanged?.Invoke(_current.Id);
            return true;
        }

        private bool InternalOpen(ScreenId id, bool push, bool allowFallback)
        {
            if (id == ScreenId.None)
            {
                Debug.LogWarning("[UINavigationService] Empty screen id.");
                return false;
            }

            if (_current != null && _current.EnumId == id) return true;

            if (!registry.TryGet(id, out var next))
            {
                // Try fallback by enum first then string
                var fb = fallbackScreen != ScreenId.None ? fallbackScreen.ToString() : GetFallbackId();
                if (allowFallback && !string.IsNullOrEmpty(fb) && registry.TryGet(fb, out next))
                {
                    Debug.LogWarning($"[UINavigationService] Screen '{id}' not found. Using fallback '{fb}'.");
                }
                else
                {
                    Debug.LogWarning($"[UINavigationService] Screen '{id}' not found.");
                    return false;
                }
            }

            if (_current != null)
            {
                if (push) _history.Push(_current.Id);
                _current.Hide();
            }

            _current = next;
            _current.Show();
            OnScreenChanged?.Invoke(_current.Id);
            return true;
        }
    }
}
