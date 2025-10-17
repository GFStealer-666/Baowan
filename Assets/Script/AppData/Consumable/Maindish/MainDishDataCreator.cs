using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class MainDishDataCreator : EditorWindow
{
    private string saveFolder = "Assets/Script/AppData/Consumable/Maindish";
    private string resourcesFolder = "MainDishImages"; // under Assets/Resources/

    [MenuItem("Tools/MainDish/Generate Main Dishes (จาก Preset)")]
    private static void GenerateAllMenu() => GenerateFromPresetInternal();

    // ====== PRESETS from your screenshot ======
    private static Preset[] Presets = new Preset[]
    {
        // Food1 — ข้าวผัดผักรวม (เวอร์ชันสุขภาพ)
        P(
            "ข้าวผัดผักรวม (สุขภาพ)", "Veg Fried Rice (Healthy)",
            "veg_fried_rice",
            "ข้าวกล้องสุก 1 ถ้วย, ไข่ 1 ฟอง, ผักรวม (ข้าวโพด แครอท ถั่วลันเตา) 1 ถ้วย, น้ำมันพืช 1 ช้อนชา, ซีอิ๊วขาวเล็กน้อย",
            "1) เจียวไข่ใส่พอจับตัว แล้วตักพัก\n2) ผัดผักด้วยน้ำมันน้อย ๆ จนสุกกรอบ\n3) ใส่ข้าว ไข่ ปรุงรสด้วยซีอิ๊วขาว ผัดให้เข้ากัน",
            0f,
            "ใช้ข้าวกล้อง/น้ำมันน้อย ช่วยเพิ่มไฟเบอร์และลดแคลอรีจากไขมัน"
        ),

        // Food2 — สลัดไข่ต้ม
        P(
            "สลัดไข่ต้ม", "Boiled Egg Salad",
            "egg_salad",
            "ไข่ต้มสุก 1 ฟอง, ผักสลัดรวม, มะเขือเทศเชอรี่, แตงกวา, น้ำสลัดโยเกิร์ตไขมันต่ำ 1 ช้อนโต๊ะ",
            "1) หั่นไข่ต้มเป็นชิ้นพอดีคำ\n2) จัดผักสลัดในชาม ราดน้ำสลัดเล็กน้อย\n3) วางไข่ต้มด้านบน คลุกเบา ๆ ก่อนทาน",
            0f,
            "ไข่ให้โปรตีนสูงและอิ่มนาน น้ำสลัดโยเกิร์ตช่วยลดพลังงานเมื่อเทียบมายองเนส"
        ),

        // Food3 — ต้มจืดผักรวม
        P(
            "ต้มจืดผักรวม", "Mixed Veg Clear Soup",
            "clear_soup_mixed_veg",
            "ผักรวม (ผักกาด กะหล่ำ แครอท เห็ด) 1 ถ้วยครึ่ง, เต้าหู้ไข่/เต้าหู้ขาว 1 หลอด, กระเทียมเจียวเล็กน้อย, ซีอิ๊วขาว/เกลือเล็กน้อย",
            "1) ต้มน้ำให้เดือด\n2) ใส่ผักและเต้าหู้ เคี่ยวจนผักนุ่ม\n3) ปรุงรสอ่อน ๆ ชิมรส",
            0f,
            "อาหารน้ำซุปไขมันต่ำ ช่วยให้ทานผักได้มากขึ้นและคุมแคลอรี"
        ),

        // Food4 — โยเกิร์ตผลไม้สด
        P(
            "โยเกิร์ตผลไม้สด", "Yogurt with Fresh Fruits",
            "yogurt_fruit_bowl",
            "โยเกิร์ตธรรมดา (ไม่หวาน) 1 ถ้วย, ผลไม้สดตามชอบ (กล้วย/แอปเปิล/เบอร์รี) รวม ~1 ถ้วย, ธัญพืชอบกรอบไม่หวาน 1 ช้อนโต๊ะ (ไม่จำเป็น)",
            "1) ใส่โยเกิร์ตเป็นฐาน\n2) โรยผลไม้สดและธัญพืชเล็กน้อย\n3) เสิร์ฟทันที",
            0f,
            "โปรไบโอติกจากโยเกิร์ต ดีต่อทางเดินอาหาร; เลือกโยเกิร์ตไม่หวานเพื่อลดน้ำตาล"
        ),

        // Food5 — ผัดกุ้งและเห็ด (น้ำมันน้อย)
        P(
            "ผัดกุ้งและเห็ด", "Shrimp & Mushroom Stir-fry",
            "shrimp_mushroom_stirfry",
            "กุ้งสด 7–8 ตัว, เห็ดต่างๆ 1 ถ้วย, กระเทียมสับ, น้ำมัน 1 ช้อนชา, น้ำปลา/ซีอิ๊วขาว/พริกไทยเล็กน้อย, ผักชี/ต้นหอม",
            "1) เจียวกระเทียมไฟอ่อนด้วยน้ำมันน้อย\n2) ใส่กุ้ง ผัดจนเริ่มสุก ใส่เห็ดผัดต่อ\n3) ปรุงรส เคล้าให้เข้ากัน ปิดไฟแล้วโรยผักชี",
            0f,
            "ใช้น้ำมันน้อยและปรุงรสพอดี ลดโซเดียมและพลังงานจากไขมัน"
        ),

        // Food6 — ไข่อะโวคาโด
        P(
            "ไข่อะโวคาโด", "Avocado & Egg",
            "avocado_egg",
            "ไข่ไก่ 1 ฟอง, อะโวคาโด 1 ผล, เกลือ/พริกไทยเล็กน้อย, น้ำมะนาวนิดหน่อย",
            "1) ต้ม/ลวกไข่ให้สุกตามชอบ\n2) หั่นอะโวคาโด วางไข่ด้านบน ปรุงรสเบา ๆ",
            0f,
            "ไม่มีน้ำตาล ไขมันไม่อิ่มตัวจากอะโวคาโดดีต่อหัวใจ ช่วยอิ่มนาน"
        ),
    };

    // ====== generator ======
    public static void OpenWindow()
    {
        var w = GetWindow<MainDishDataCreator>("Main Dish Generator");
        w.minSize = new Vector2(420, 220);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Auto Main Dish ScriptableObject Generator", EditorStyles.boldLabel);
        saveFolder = EditorGUILayout.TextField("Save Folder", saveFolder);
        resourcesFolder = EditorGUILayout.TextField("Resources Subfolder", resourcesFolder);
        if (GUILayout.Button("Generate From Preset")) GenerateFromPresetInternal(saveFolder, resourcesFolder);
    }

    private static void GenerateFromPresetInternal(string saveFolder = "Assets/ScriptableObjects/MainDishes/",
                                                   string resourcesSub = "MainDishImages")
    {
        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

        int created = 0;
        foreach (var p in Presets) { CreateSO(saveFolder, resourcesSub, p); created++; }

        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Main Dish Generator", $"✅ Created/Updated {created} dishes", "OK");
    }

    private static void CreateSO(string saveFolder, string resourcesSub, Preset p)
    {
        string assetPath = Path.Combine(saveFolder, Slug(p.en) + ".asset").Replace("\\", "/");
        var so = AssetDatabase.LoadAssetAtPath<MainDishDataSO>(assetPath);
        bool isNew = false;
        if (so == null) { so = ScriptableObject.CreateInstance<MainDishDataSO>(); isNew = true; }

        so.dishNameTH = p.th;
        so.dishNameEN = p.en;
        so.ingredients = p.ing;
        so.method = p.method;
        so.calories = p.kcal;
        so.tip = p.tip;

        if (!string.IsNullOrWhiteSpace(p.img))
        {
            string resPath = string.IsNullOrWhiteSpace(resourcesSub) ? p.img : $"{resourcesSub}/{p.img}";
            var sprite = Resources.Load<Sprite>(resPath);
            if (sprite != null) so.image = sprite;
            else Debug.LogWarning($"[MainDish] sprite not found: {resPath}");
        }

        if (isNew) AssetDatabase.CreateAsset(so, assetPath);
        else EditorUtility.SetDirty(so);
    }

    // helpers
    private static Preset P(string th, string en, string img, string ing, string method, float kcal, string tip)
        => new Preset(th, en, img, ing, method, kcal, tip);

    private static string Slug(string s)
    {
        s = s.ToLowerInvariant();
        s = Regex.Replace(s, @"[^a-z0-9]+", "_");
        s = Regex.Replace(s, @"_+", "_").Trim('_');
        return string.IsNullOrEmpty(s) ? "main_dish" : s;
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
