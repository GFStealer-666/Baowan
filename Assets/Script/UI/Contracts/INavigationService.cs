using System;

namespace BW.UI
{
    public interface INavigationService
    {
        string CurrentId { get; }
        ScreenId Current { get; }
        event Action<string> OnScreenChanged;
        bool GoTo(string id, bool pushToHistory = true);
        bool GoTo(ScreenId id, bool pushToHistory = true);
        bool Back();
        void ResetTo(string id); // clears history and opens id
        void ResetTo(ScreenId id); // enum overload
    }
}
