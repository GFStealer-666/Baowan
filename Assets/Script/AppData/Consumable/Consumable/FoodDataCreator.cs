using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class FoodDataCreator : EditorWindow
{
    private string saveFolder = "Assets/Script/AppData/ScripableObject/Drink";
    private string resourcesFolder = "FoodImages";

    [MenuItem("Tools/Food/Generate All Thai Foods (from screenshots)")]
    private static void GenerateAllMenu() => GenerateFromPresetInternal();

    [MenuItem("Tools/Food/Open Generator Window")]
    public static void OpenWindow()
    {
        var w = GetWindow<FoodDataCreator>("Food Generator");
        w.minSize = new Vector2(420, 220);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Auto Food ScriptableObject Generator", EditorStyles.boldLabel);
        saveFolder = EditorGUILayout.TextField("Save Folder", saveFolder);
        resourcesFolder = EditorGUILayout.TextField("Resources Subfolder", resourcesFolder);
        if (GUILayout.Button("Generate All"))
            GenerateFromPresetInternal(saveFolder, resourcesFolder);
    }

    // === Preset data (ALL screenshots combined) ===
    private static Preset[] Presets = new Preset[]
{
     P("ผัดบวบใส่ไข่","Phat Buap Sai Khai","phat_buap_saikhai",
      ConsumeType.SideDish,45,10,280,8f,70f,600f,10f,10f),

    P("ผัดมะเขือยาวหมูสับ","Phat Makhuea Yao Mu Sap","phat_makhuea_yao_musap",
      ConsumeType.SideDish,50,12,320,12f,70f,600f,10f,10f),

    P("ปลานึ่งมะนาว","Pla Nueng Manao","pla_nueng_manao",
      ConsumeType.SideDish,0,0,200,7f,70f,700f,9f,20f),

    P("ข้าวหมูกรอบ","Khao Mu Krop","khao_mu_krop",
      ConsumeType.Rice,65,35,650,22f,90f,1200f,70f,25f),

    P("ไข่ดาว","Khai Dao","khai_dao",
      ConsumeType.SideDish,0,0,90,7f,80f,80f,0f,6f),

    P("ไข่เจียว","Khai Chiao","khai_chiao",
      ConsumeType.SideDish,20,5,250,18f,210f,440f,1f,12f),

    P("ยำหมูยอ","Yam Mu Yo","yam_mu_yo",
      ConsumeType.SideDish,45,10,280,8f,70f,850f,6f,12f),

    // ----- From second image -----
    P("ปลาดุกย่าง","Pladuk Yang","pladuk_yang",
      ConsumeType.SideDish,0,0,180,8f,70f,90f,0f,25f),

    P("ข้าวผัดปู","Khaophat Pu","khaophat_pu",
      ConsumeType.Rice,65,35,550,14f,70f,1000f,75f,16f),

    P("ข้าวมันไก่ต้ม","Khaomankai Tom","khaomankai_tom",
      ConsumeType.Rice,70,40,600,25f,75f,700f,75f,20f),

    P("ข้าวมันไก่ทอด","Khaomankai Thot","khaomankai_thot",
      ConsumeType.Rice,65,35,750,40f,80f,750f,75f,23f),

    P("ข้าวหมกไก่","Khaomok Kai","khaomok_kai",
      ConsumeType.Rice,65,35,650,20f,75f,900f,70f,20f),

    P("ไข่พะโล้","Khai Phalo","khai_phalo",
      ConsumeType.SideDish,30,5,200,5f,200f,500f,19f,12f),

    P("แกงจืดเต้าหู้หมูสับ","Kaengchued Taohu Mu Sap","kaengchued_taohu_musap",
      ConsumeType.SideDish,30,8,200,8f,75f,700f,10f,12f),
};


    // ==== Generation Core ====
    private static void GenerateFromPresetInternal(string saveFolder = "Assets/ScriptableObjects/Food/",
                                                   string resourcesSub = "FoodImages")
    {
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        int created = 0;
        foreach (var p in Presets)
            created += CreateSO(saveFolder, resourcesSub, p) ? 1 : 0;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Food Generator",
            $"✅ Created or updated {created} food assets!\nSaved to: {saveFolder}", "OK");
    }

    private static bool CreateSO(string saveFolder, string resourcesSub, Preset p)
    {
        string fileName = Slug(p.en) + ".asset";
        string assetPath = Path.Combine(saveFolder, fileName).Replace("\\", "/");
        FoodDataSO so = AssetDatabase.LoadAssetAtPath<FoodDataSO>(assetPath);
        bool isNew = false;
        if (so == null) { so = ScriptableObject.CreateInstance<FoodDataSO>(); isNew = true; }

        so.foodNameTH = p.th;
        so.foodNameEN = p.en;

        so.consumeType = p.consume;
        so.glycemicIndex = p.gi;
        so.glycemicLoad = p.gl;
        so.calories = p.kcal;
        so.totalFat = p.fat;
        so.cholesterol = p.chol;
        so.sodium = p.sod;
        so.carbohydrate = p.carb;
        so.protein = p.pro;

        if (!string.IsNullOrWhiteSpace(p.img))
        {
            string resPath = string.IsNullOrWhiteSpace(resourcesSub) ? p.img : $"{resourcesSub}/{p.img}";
            var sprite = Resources.Load<Sprite>(resPath);
            if (sprite != null) so.foodImage = sprite;
            else Debug.LogWarning($"⚠️ Sprite not found for {p.en}: {resPath}");
        }

        if (isNew) AssetDatabase.CreateAsset(so, assetPath);
        else EditorUtility.SetDirty(so);
        return true;
    }

    // === Helper methods ===


    private static string Slug(string s)
    {
        s = s.ToLowerInvariant();
        s = Regex.Replace(s, @"[^a-z0-9]+", "_");
        s = Regex.Replace(s, @"_+", "_").Trim('_');
        return string.IsNullOrEmpty(s) ? "food" : s;
    }

    private static Preset P(string th, string en, string img, ConsumeType c,
                        float gi, float gl, float kcal, float fat,
                        float chol, float sod, float carb, float pro)
    => new Preset(th, en, img, c, gi, gl, kcal, fat, chol, sod, carb, pro);

private struct Preset
{
    public string th, en, img;
    public ConsumeType consume;
    public float gi, gl, kcal, fat, chol, sod, carb, pro;
    public Preset(string th, string en, string img, ConsumeType consume,
                  float gi, float gl, float kcal, float fat,
                  float chol, float sod, float carb, float pro)
    {
        this.th = th; this.en = en; this.img = img; this.consume = consume;
        this.gi = gi; this.gl = gl; this.kcal = kcal; this.fat = fat;
        this.chol = chol; this.sod = sod; this.carb = carb; this.pro = pro;
    }
}
}
