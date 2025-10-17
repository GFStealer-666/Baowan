using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class DrinkDataCreator : EditorWindow
{
    private string saveFolder = "Assets/ScriptableObjects/Drinks/";
    private string resourcesFolder = "DrinkImages";

    [MenuItem("Tools/Drink/Generate Drinks")]
    private static void GenerateAllMenu() => GenerateFromPresetInternal();

    private static Preset[] Presets = new Preset[]
{
    // Drink 1 — ชาเขียวไม่หวาน
    P(
        "ชาเขียวไม่หวาน",
        "Unsweetened Green Tea",
        "green_tea_unsweet",
        "ใบชาเขียวแบบใบแห้ง ~1 ช้อนชา, น้ำร้อน ~200 ml",
        "ต้มน้ำ ~80°C ใส่ใบชา พัก ~3 นาที แล้วกรองดื่ม",
        0f, 0f, 0f,
        "มีแคทิชิน (Catechins) ต้านอนุมูลอิสระ ช่วยลด LDL; ไม่มีน้ำตาล/แคลอรีต่ำ; ดื่มอุ่นช่วยให้ผ่อนคลาย"
    ),

    // Drink 2 — เก๊กฮวย
    P(
        "เก๊กฮวย",
        "Chrysanthemum Tea",
        "chrysanthemum_tea",
        "ดอกเก๊กฮวยแห้ง ~5 ดอก, น้ำ ~500 ml",
        "ต้มน้ำให้เดือด ใส่ดอกเก๊กฮวย ต้มไฟอ่อน ~10–15 นาที กรอง ดื่มอุ่นหรือเย็น",
        0f, 0f, 0f,
        "ช่วยให้สดชื่น/ผ่อนคลาย; เครื่องดื่มไร้น้ำตาลเมื่อไม่เติมหวาน; จิบบรรเทาคอแห้งได้"
    ),

    // Drink 3 — กาแฟดำ
    P(
        "กาแฟดำ",
        "Black Coffee",
        "black_coffee",
        "กาแฟคั่วบด ~1 ช้อนโต๊ะ, น้ำร้อน ~200 ml",
        "ชงด้วย Drip/French Press หรือ Moka Pot; ไม่เติมน้ำตาล ครีม หรือนมข้น",
        0f, 0f, 2f,
        "คาเฟอีนช่วยเพิ่มความตื่นตัวและสมาธิ; แคลอรีต่ำมากเมื่อไม่เติมหวาน; ควรหลีกเลี่ยงช่วงท้องว่างหากไวต่อกรด"
    ),

    // Drink 4 — น้ำมะนาวแบบไม่หวาน
    P(
        "น้ำมะนาวแบบไม่หวาน",
        "Unsweetened Lime Water",
        "lime_water_unsweet",
        "มะนาว 1/2 ลูก, น้ำ ~200 ml",
        "บีบน้ำมะนาว เติมน้ำตามชอบ ดื่มแบบอุณหภูมิห้องหรือเย็น",
        0f, 0f, 5f,
        "มีวิตามิน C ช่วยให้สดชื่น; ไม่มีน้ำตาล; จิบบำรุงคอระหว่างวัน"
    ),

    // Drink 5 — น้ำกระเจี๊ยบ (ไม่หวาน)
    P(
        "น้ำกระเจี๊ยบ",
        "Roselle Tea (Unsweetened)",
        "roselle_tea_unsweet",
        "กลีบกระเจี๊ยบแห้ง ~10 ดอก, น้ำ ~1 ลิตร",
        "ล้างกลีบ ต้มไฟอ่อน ~10–15 นาที พักให้ใบสีออกครบแล้วกรองดื่ม",
        0f, 0f, 5f,
        "มีแอนโธไซยานิน ต้านอนุมูลอิสระ; สดชื่น เปรี้ยวอมฝาดเล็กน้อย; แคลอรีต่ำเมื่อไม่เติมหวาน"
    ),

    // Drink 6 — นม Low-Fat หรือถั่วเหลืองไม่หวาน
    P(
        "นม Low-Fat หรือถั่วเหลืองไม่หวาน",
        "Low-Fat Milk or Unsweetened Soy Milk",
        "lowfat_milk_or_unsweet_soymilk",
        "นมพร่องมันเนย (Low-Fat) หรือ น้ำนมถั่วเหลืองไม่หวาน ~200 ml",
        "เขย่าก่อนดื่ม; ดื่มหลังกินอาหารหรือระหว่างวัน",
        0f, 0f, 100f,   // ≈100 kcal ต่อ 200 ml โดยคร่าว ๆ
        "เป็นแหล่งโปรตีนและแคลเซียม; ถั่วเหลืองมีไอโซฟลาโวน/ไฟโตเอสโตรเจน; อิ่มนานเมื่อไม่เติมน้ำตาล"
    ),
};


    private static void GenerateFromPresetInternal(string saveFolder = "Assets/ScriptableObjects/Drinks/",
                                                   string resourcesSub = "DrinkImages")
    {
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        int created = 0;
        foreach (var p in Presets)
        {
            string fileName = Slug(p.en) + ".asset";
            string assetPath = Path.Combine(saveFolder, fileName).Replace("\\", "/");
            DrinkDataSO so = AssetDatabase.LoadAssetAtPath<DrinkDataSO>(assetPath);
            bool isNew = false;
            if (so == null) { so = ScriptableObject.CreateInstance<DrinkDataSO>(); isNew = true; }

            so.drinkNameTH = p.th;
            so.drinkNameEN = p.en;
            so.ingredient = p.ing;
            so.method = p.method;
            so.calories = p.cal;
            so.glycemicIndex = p.gi;
            so.glycemicLoad = p.gl;
            so.benefit = p.benefit;

            if (!string.IsNullOrWhiteSpace(p.img))
            {
                string resPath = string.IsNullOrWhiteSpace(resourcesSub) ? p.img : $"{resourcesSub}/{p.img}";
                var sprite = Resources.Load<Sprite>(resPath);
                if (sprite != null) so.drinkImage = sprite;
                else Debug.LogWarning($"⚠️ Image not found for {p.en}: {resPath}");
            }

            if (isNew) AssetDatabase.CreateAsset(so, assetPath);
            else EditorUtility.SetDirty(so);
            created++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Drink Generator", $"✅ Created or updated {created} drinks!", "OK");
    }

    private static Preset P(string th, string en, string img, string ing, string method,
                            float gi, float gl, float cal, string benefit)
        => new Preset(th, en, img, ing, method, gi, gl, cal, benefit);

    private static string Slug(string s)
    {
        s = s.ToLowerInvariant();
        s = Regex.Replace(s, @"[^a-z0-9]+", "_");
        s = Regex.Replace(s, @"_+", "_").Trim('_');
        return string.IsNullOrEmpty(s) ? "drink" : s;
    }

    private struct Preset
    {
        public string th, en, img, ing, method, benefit;
        public float gi, gl, cal;
        public Preset(string th, string en, string img, string ing, string method,
                      float gi, float gl, float cal, string benefit)
        {
            this.th = th; this.en = en; this.img = img;
            this.ing = ing; this.method = method;
            this.gi = gi; this.gl = gl; this.cal = cal;
            this.benefit = benefit;
        }
    }
}
