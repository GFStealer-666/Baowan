using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class TimeInputFormatter : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;

    private bool _isUpdating;
    private string _lastValid = "";

    private void Awake()
    {
        if (!input) input = GetComponent<TMP_InputField>();
        input.onValueChanged.AddListener(OnValueChanged);

        // Start state
        _lastValid = input.text;
    }

    private void OnDestroy()
    {
        if (input != null)
            input.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(string raw)
    {
        if (_isUpdating) return;

        string formatted = FormatTime(raw);

        // If invalid (hour>23 or minute>59) → revert to last valid
        if (formatted == null)
            formatted = _lastValid;
        else
            _lastValid = formatted;

        if (formatted != raw)
        {
            _isUpdating = true;
            input.text = formatted;
            input.caretPosition = formatted.Length;
            _isUpdating = false;
        }
    }

    /// <summary>
    /// Returns formatted string or null if invalid.
    /// Rules:
    ///  - Only digits and one ':'
    ///  - Auto insert ':' when more than 2 digits
    ///  - Hour (first 2 digits) < 24
    ///  - Minute (last 2 digits) < 60
    /// </summary>
    private string FormatTime(string raw)
    {
        if (string.IsNullOrEmpty(raw))
            return "";

        // 1) Keep only digits and ':'
        var filtered = new StringBuilder();
        foreach (char c in raw)
        {
            if (char.IsDigit(c) || c == ':')
                filtered.Append(c);
        }

        string t = filtered.ToString();

        // 2) Allow only one ':' (keep the first one)
        int firstColon = t.IndexOf(':');
        if (firstColon >= 0)
        {
            var clean = new StringBuilder();
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] == ':' && i != firstColon) continue;
                clean.Append(t[i]);
            }
            t = clean.ToString();
        }

        // 3) If no colon and >2 digits → auto insert HH:MM
        if (t.IndexOf(':') < 0)
        {
            string digits = new string(t.Where(char.IsDigit).ToArray());

            if (digits.Length <= 2)
            {
                // Only hour part so far
                // If 2 digits and >23 -> invalid
                if (digits.Length == 2 &&
                    int.TryParse(digits, out int h) &&
                    h > 23)
                    return null;

                return digits;
            }
            else
            {
                // More than 2 digits -> split into HH and MM
                string hPart = digits.Substring(0, 2);
                string mPart = digits.Substring(2, Mathf.Min(2, digits.Length - 2));

                // Validate hour
                if (int.TryParse(hPart, out int h) && h > 23)
                    return null;

                // Validate minute (only when 2 digits)
                if (mPart.Length == 2 &&
                    int.TryParse(mPart, out int m) &&
                    m > 59)
                    return null;

                return hPart + ":" + mPart;
            }
        }
        else
        {
            // Already has a colon
            if (t.Length > 5)
                t = t.Substring(0, 5);      // Max "HH:MM"

            int colon = t.IndexOf(':');
            string hPart = t.Substring(0, colon);
            string mPart = colon + 1 < t.Length ? t.Substring(colon + 1) : "";

            // Invalid if hour has >2 digits
            if (hPart.Length > 2) return null;
            // Invalid if minute has >2 digits
            if (mPart.Length > 2) return null;

            // Validate hour when 2 digits
            if (hPart.Length == 2 &&
                int.TryParse(hPart, out int h) &&
                h > 23)
                return null;

            // Validate minute when 2 digits
            if (mPart.Length == 2 &&
                int.TryParse(mPart, out int m) &&
                m > 59)
                return null;

            return t;
        }
    }
}
