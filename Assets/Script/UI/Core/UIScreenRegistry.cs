using System.Collections.Generic;
using UnityEngine;

namespace BW.UI
{
    /// <summary>Holds and resolves all screens (SRP: discovery/lookup only).</summary>
    public sealed class UIScreenRegistry : MonoBehaviour
    {
        [SerializeField] private List<CanvasScreen> screens = new();

        // Support both enum and string lookups for compatibility
        private readonly Dictionary<ScreenId, IUIScreen> _mapByEnum = new();
        private readonly Dictionary<string, IUIScreen> _mapByString = new(System.StringComparer.Ordinal);

        public bool TryGet(ScreenId id, out IUIScreen screen) => _mapByEnum.TryGetValue(id, out screen);
        public bool TryGet(string id, out IUIScreen screen)
        {
            screen = null;
            if (string.IsNullOrEmpty(id)) return false;

            // Try parse to enum first (case-insensitive)
            if (System.Enum.TryParse<ScreenId>(id, true, out var sid) && sid != ScreenId.None)
            {
                if (_mapByEnum.TryGetValue(sid, out screen)) return true;
            }

            // Fallback to the registered string id
            return _mapByString.TryGetValue(id, out screen);
        }

        private void Awake()
        {
            _mapByEnum.Clear();
            _mapByString.Clear();
            foreach (var s in screens)
            {
                if (s == null) continue;

                // Register by enum key (if not None)
                if (s.EnumId != ScreenId.None)
                {
                    if (_mapByEnum.ContainsKey(s.EnumId))
                    {
                        Debug.LogWarning($"Duplicate Screen enum '{s.EnumId}' on {s.name}. Skipping enum registration.");
                    }
                    else
                    {
                        _mapByEnum.Add(s.EnumId, s);
                    }
                }

                // Register by string id (s.Id)
                if (string.IsNullOrEmpty(s.Id)) continue;
                if (_mapByString.ContainsKey(s.Id))
                {
                    Debug.LogWarning($"Duplicate Screen Id '{s.Id}' on {s.name}. Skipping string registration.");
                }
                else
                {
                    _mapByString.Add(s.Id, s);
                    Debug.Log($"[UIScreenRegistry] Registered screen id='{s.Id}' (component {s.name})");
                }
            }
        }
    }
}
