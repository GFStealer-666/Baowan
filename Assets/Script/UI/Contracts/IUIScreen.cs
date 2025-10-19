using UnityEngine;

namespace BW.UI
{
    /// <summary>Abstraction of a UI screen.</summary>
    public interface IUIScreen
    {
        string Id { get; }                 // Unique string id, e.g., "Home", "Nutrition"
    ScreenId EnumId { get; }         // Enum-backed id when available
        GameObject Root { get; }           // Root object (usually the Canvas)
        void Show();
        void Hide();
        bool IsVisible { get; }
    }
}
