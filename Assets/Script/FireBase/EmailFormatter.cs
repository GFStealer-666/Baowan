using System.Text.RegularExpressions;
using UnityEngine;

public class EmailFormatter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Singleton instance
    public static EmailFormatter I { get; private set; }

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Cleans an email string: trims, removes line breaks, common invisible unicode characters,
    /// and collapses repeated whitespace into single spaces.
    /// This is exposed as a static method so it can be called without a reference to the instance.
    /// </summary>
    public static string CleanEmail(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return raw;

        // Trim normal whitespace and common control chars
        string s = raw.Trim();

        // Remove line breaks
        s = s.Replace("\r", "").Replace("\n", "");

        // Remove common invisible Unicode characters that appear with copy/paste
        char[] invisibles = new char[] { '\u200B', '\u200C', '\u200D', '\uFEFF', '\u00A0' };
        foreach (var c in invisibles) s = s.Replace(c.ToString(), "");

        // Also collapse multiple spaces (optional)
        s = Regex.Replace(s, @"\s+", " ");

        return s;
    }

    public static bool LooksLikeEmail(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        // Simple, practical regex — not perfect RFC but OK for client-side check
        return System.Text.RegularExpressions.Regex.IsMatch(
            s,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }

    // Instance wrapper if you prefer instance calls
    public string CleanEmailInstance(string raw) => CleanEmail(raw);
}
