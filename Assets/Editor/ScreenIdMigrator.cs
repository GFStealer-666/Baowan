#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using BW.UI;
public static class ScreenIdMigrator
{
    [MenuItem("Tools/Migrate/Update ScreenIds")]
    public static void UpdateScreenIds()
    {
        // maps legacy string ids (variants) -> new enum values
        var map = new Dictionary<string, ScreenId>(System.StringComparer.OrdinalIgnoreCase)
        {
            { "Recommand", ScreenId.RecommendMenu },
            { "RecommendMenu", ScreenId.RecommendMenu },
            { "RecommendedMenu", ScreenId.RecommendMenu },
            { "MenuList", ScreenId.MenuList },
            { "MaincouseDetail", ScreenId.MaincourseDetail },
            { "MaincourseDetail", ScreenId.MaincourseDetail },
            { "AppitizerDetail", ScreenId.AppetizerDetail },
            { "AppetizerDetail", ScreenId.AppetizerDetail },
            { "GIGLDetail", ScreenId.MenuList },
            { "FoodDetailCal", ScreenId.FoodDetail },
            { "FoodDetail", ScreenId.FoodDetail }
        };

        var all = Resources.FindObjectsOfTypeAll<CanvasScreen>();
        int changed = 0;

        foreach (var s in all)
        {
            if (s == null) continue;

            // Skip objects not part of project assets/scenes (playmode editor instances)
            var assetPath = AssetDatabase.GetAssetPath(s);
            if (string.IsNullOrEmpty(assetPath) && !EditorUtility.IsPersistent(s))
            {
                // still try to fix scene instances too (they may be in open scenes)
            }

            // Read the serialized legacy 'id' field if present; fallback to computed Id
            var so = new SerializedObject(s);
            var legacyProp = so.FindProperty("id");
            string legacy = null;
            if (legacyProp != null)
                legacy = legacyProp.stringValue;
            if (string.IsNullOrWhiteSpace(legacy))
                legacy = s.Id; // computed Id string (may be enum or legacy)

            if (string.IsNullOrWhiteSpace(legacy)) continue;

            var key = legacy.Replace(" ", "").Trim();
            if (!map.TryGetValue(key, out var target)) continue;

            var enumProp = so.FindProperty("screenId");
            if (enumProp == null) continue; // nothing to set

            if (enumProp.enumValueIndex != (int)target)
            {
                enumProp.enumValueIndex = (int)target;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(s);
                Debug.Log($"[ScreenIdMigrator] Set {s.gameObject.name} -> {target}");
                changed++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[ScreenIdMigrator] Updated {changed} objects.");
    }
}
#endif
