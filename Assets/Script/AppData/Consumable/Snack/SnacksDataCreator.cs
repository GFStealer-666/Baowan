using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class SnacksDataCreator : EditorWindow
{
    private string saveFolder = "Assets/Script/AppData/ScripableObject/Snack";
    private string resourcesFolder = "SnackImages";

    [MenuItem("Tools/Snack/Generate Snacks (จาก Preset)")]
    private static void GenerateAllMenu() => GenerateFromPresetInternal();

    private static Preset[] Presets = new Preset[]
    {
        // Snack 1 — ถั่วต่างๆ
        P(
            "ถั่วต่าง ๆ (อัลมอนด์, วอลนัท, เม็ดมะม่วงหิมพานต์)",
            "Mixed Nuts (Almond, Walnut, Cashew)",
            "mixed_nuts",
            "ถั่วอบไม่เค็ม ~1 กำมือ (30 กรัม)",
            "ทานเล่นระหว่างมื้อ หรือเพิ่มในโยเกิร์ต/สลัด",
            0f,
            "มีไขมันดี โปรตีนสูง ช่วยให้อิ่มนาน; มีใยอาหาร; แต่อย่าทานเกิน 1 กำมือต่อวันเพราะพลังงานสูง"
        ),

        // Snack 2 — แครอทแตงกวา
        P(
            "แตงกวา แครอท",
            "Cucumber & Carrot Sticks",
            "carrot_cucumber_stick",
            "หั่นเป็นแท่งพอดีคำ",
            "แช่เย็นก่อนเสิร์ฟให้อร่อยกรอบ",
            0f,
            "ให้ไฟเบอร์สูง ช่วยระบบขับถ่าย; แคลอรีต่ำ เหมาะเป็นของว่างยามบ่าย"
        ),

        // Snack 3 — โยเกิร์ต Greek แบบไม่หวาน
        P(
            "โยเกิร์ต Greek Yogurt แบบไม่หวาน",
            "Greek Yogurt (Unsweetened)",
            "greek_yogurt_unsweet",
            "โยเกิร์ตรสธรรมชาติ 1 ถ้วย",
            "แช่เย็นพร้อมเสิร์ฟ หรือทานคู่ผลไม้สด",
            0f,
            "มีโปรตีนสูง ย่อยง่าย; ไม่มีน้ำตาลเติม; ช่วยเสริมโปรไบโอติกดีต่อระบบลำไส้"
        ),

        // Snack 4 — ถั่วลิสงต้ม
        P(
            "ถั่วลิสงต้ม",
            "Boiled Peanuts",
            "boiled_peanuts",
            "ถั่วลิสงดิบ 1 ถ้วย",
            "ต้มจนสุกนิ่ม ประมาณ 45–60 นาที",
            0f,
            "ให้โปรตีนและไขมันดี; มีแมกนีเซียมสูง ช่วยบำรุงหัวใจ; แต่อย่าทานมากเพราะโซเดียมสูง"
        ),

        // Snack 5 — ไข่ต้ม
        P(
            "ไข่ต้ม",
            "Boiled Eggs",
            "boiled_eggs",
            "ไข่ไก่ 1–2 ฟอง",
            "ต้มในน้ำเดือด 5–10 นาที",
            0f,
            "โปรตีนคุณภาพสูง; มีโคลีนช่วยบำรุงสมอง; ไม่ควรเกินวันละ 1–2 ฟอง"
        ),

        // Snack 6 — แอปเปิ้ลเขียว / ผลไม้ GI ต่ำ
        P(
            "แอปเปิ้ลเขียว หรือผลไม้ GI ต่ำ",
            "Green Apple or Low-GI Fruits",
            "green_apple_low_gi",
            "ผลไม้เช่น แอปเปิ้ลเขียว แก้วมังกร ฝรั่ง",
            "หั่นชิ้นพอดีคำ รับประทานสด",
            0f,
            "มีวิตามิน C และไฟเบอร์สูง; GI ต่ำ เหมาะกับผู้ควบคุมน้ำตาลในเลือด; สดชื่นระหว่างวัน"
        ),
    };

    // ===== Generator Core =====
    private static void GenerateFromPresetInternal(string saveFolder = "Assets/ScriptableObjects/Snacks/",
                                                   string resourcesSub = "SnackImages")
    {
        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

        int created = 0;
        foreach (var p in Presets)
        {
            string fileName = Slug(p.en) + ".asset";
            string assetPath = Path.Combine(saveFolder, fileName).Replace("\\", "/");
            SnacksDataSO so = AssetDatabase.LoadAssetAtPath<SnacksDataSO>(assetPath);
            bool isNew = false;
            if (so == null) { so = ScriptableObject.CreateInstance<SnacksDataSO>(); isNew = true; }

            so.snackNameTH = p.th;
            so.snackNameEN = p.en;
            so.ingredient = p.ing;
            so.method = p.method;
            so.calories = p.kcal;
            so.tip = p.tip;

            if (!string.IsNullOrWhiteSpace(p.img))
            {
                string resPath = string.IsNullOrWhiteSpace(resourcesSub) ? p.img : $"{resourcesSub}/{p.img}";
                var sprite = Resources.Load<Sprite>(resPath);
                if (sprite != null) so.image = sprite;
                else Debug.LogWarning($"⚠️ Image not found: {resPath}");
            }

            if (isNew) AssetDatabase.CreateAsset(so, assetPath);
            else EditorUtility.SetDirty(so);
            created++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Snack Generator", $"✅ Created or updated {created} snacks!", "OK");
    }

    // Helper structs
    private static Preset P(string th, string en, string img, string ing, string method, float kcal, string tip)
        => new Preset(th, en, img, ing, method, kcal, tip);

    private static string Slug(string s)
    {
        s = s.ToLowerInvariant();
        s = Regex.Replace(s, @"[^a-z0-9]+", "_");
        s = Regex.Replace(s, @"_+", "_").Trim('_');
        return string.IsNullOrEmpty(s) ? "snack" : s;
    }

    private struct Preset
    {
        public string th, en, img, ing, method, tip;
        public float kcal;
        public Preset(string th, string en, string img, string ing, string method, float kcal, string tip)
        {
            this.th = th; this.en = en; this.img = img;
            this.ing = ing; this.method = method; this.kcal = kcal; this.tip = tip;
        }
    }
}
