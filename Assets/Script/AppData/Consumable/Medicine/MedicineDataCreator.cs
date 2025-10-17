using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class MedicineDataCreator : EditorWindow
{
    private string saveFolder = "Assets/Script/AppData/ScripableObject/Medicine";
    private string resourcesFolder = "MedicineImages";

    [MenuItem("Tools/Health/Generate Medicines (จาก Preset)")]
    private static void GenerateAllMenu() => GenerateFromPresetInternal();

    private static Preset[] Presets = new Preset[]
    {
        // Medicine 1 — Metformin
        P(
            "เมตฟอร์มิน",
            "Metformin",
            "metformin",
            "Biguanides",
            "ช่วยลดระดับน้ำตาลในเลือดโดยเพิ่มความไวของร่างกายต่ออินซูลิน และลดการสร้างกลูโคสจากตับ",
            "รับประทานพร้อมอาหารเพื่อลดอาการข้างเคียงทางกระเพาะอาหาร",
            "ควรระวังภาวะกรดแลคติกในเลือดสูง โดยเฉพาะในผู้ที่มีโรคไต หรือดื่มแอลกอฮอล์มาก"
        ),

        // Medicine 2 — Sulfonylureas
        P(
            "ยากลุ่มซัลโฟนิลยูเรีย",
            "Sulfonylureas (เช่น Glimepiride, Glyburide)",
            "sulfonylureas",
            "Sulfonylureas",
            "กระตุ้นตับอ่อนให้หลั่งอินซูลินออกมาเพื่อลดระดับน้ำตาลในเลือด",
            "รับประทานก่อนอาหาร 30 นาที",
            "อาจทำให้ระดับน้ำตาลในเลือดต่ำ โดยเฉพาะในผู้สูงอายุหรือผู้ที่ไม่ได้รับประทานอาหาร"
        ),

        // Medicine 3 — DPP-4 inhibitors
        P(
            "ยากลุ่ม DPP-4 inhibitors",
            "DPP-4 Inhibitors (เช่น Sitagliptin, Saxagliptin, Linagliptin)",
            "dpp4_inhibitors",
            "DPP-4 inhibitors",
            "ยับยั้งเอนไซม์ DPP-4 ทำให้ฮอร์โมน incretin อยู่ได้นานขึ้น ส่งผลให้เพิ่มการหลั่งอินซูลิน และลดการหลั่งกลูคากอน",
            "รับประทานวันละ 1 ครั้ง พร้อมหรือไม่พร้อมอาหารก็ได้",
            "โดยทั่วไปปลอดภัย แต่ควรระวังในผู้ที่มีภาวะตับหรือไตผิดปกติ"
        ),

        // Medicine 4 — GLP-1 Agonists
        P(
            "ยากลุ่ม GLP-1 agonists",
            "GLP-1 Agonists (เช่น Liraglutide, Exenatide)",
            "glp1_agonists",
            "GLP-1 agonists",
            "เลียนแบบการทำงานของฮอร์โมน incretin เพิ่มการหลั่งอินซูลิน ลดการหลั่งกลูคากอน และช่วยให้อิ่มเร็ว",
            "มักใช้ฉีดใต้ผิวหนัง วันละ 1 ครั้ง หรือสัปดาห์ละ 1 ครั้ง ขึ้นอยู่กับยา",
            "อาจมีอาการคลื่นไส้ในช่วงแรกของการใช้ยา ห้ามใช้ในผู้ป่วยมะเร็งต่อมไทรอยด์บางชนิด"
        ),

        // Medicine 5 — SGLT-2 inhibitors
        P(
            "ยากลุ่ม SGLT-2 inhibitors",
            "SGLT-2 Inhibitors (เช่น Empagliflozin, Dapagliflozin, Canagliflozin)",
            "sglt2_inhibitors",
            "SGLT-2 inhibitors",
            "ยับยั้งการดูดซึมกลูโคสกลับที่ไต ทำให้ขับน้ำตาลออกทางปัสสาวะ",
            "รับประทานวันละ 1 ครั้ง โดยทั่วไปตอนเช้า",
            "อาจทำให้ปัสสาวะบ่อย เสี่ยงต่อการติดเชื้อทางเดินปัสสาวะ หรือขาดน้ำได้ ควรดื่มน้ำให้เพียงพอ"
        ),

        // Medicine 6 — Thiazolidinediones
        P(
            "ยากลุ่มไทอะโซลิดีนไดโอน",
            "Thiazolidinediones (เช่น Pioglitazone)",
            "thiazolidinediones",
            "Thiazolidinediones",
            "เพิ่มความไวต่ออินซูลินของเซลล์ในร่างกาย ช่วยให้ร่างกายใช้น้ำตาลได้ดีขึ้น",
            "รับประทานพร้อมอาหาร วันละ 1 ครั้ง",
            "ควรระวังในผู้ที่มีภาวะหัวใจล้มเหลว เพราะยาอาจทำให้บวมน้ำ"
        ),
    };

    // ===== Generator Core =====
    private static void GenerateFromPresetInternal(string saveFolder = "Assets/ScriptableObjects/Medicines/",
                                                   string resourcesSub = "MedicineImages")
    {
        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

        int created = 0;
        foreach (var p in Presets)
        {
            string fileName = Slug(p.en) + ".asset";
            string assetPath = Path.Combine(saveFolder, fileName).Replace("\\", "/");
            MedicineDataSO so = AssetDatabase.LoadAssetAtPath<MedicineDataSO>(assetPath);
            bool isNew = false;
            if (so == null) { so = ScriptableObject.CreateInstance<MedicineDataSO>(); isNew = true; }

            so.medNameTH = p.th;
            so.medNameEN = p.en;
            so.group = p.group;
            so.mechanism = p.mechanism;
            so.instruction = p.instruction;
            so.caution = p.caution;

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
        EditorUtility.DisplayDialog("Medicine Generator", $"✅ Created or updated {created} medicine data assets!", "OK");
    }

    private static Preset P(string th, string en, string img, string group, string mechanism, string instruction, string caution)
        => new Preset(th, en, img, group, mechanism, instruction, caution);

    private static string Slug(string s)
    {
        s = s.ToLowerInvariant();
        s = Regex.Replace(s, @"[^a-z0-9]+", "_");
        s = Regex.Replace(s, @"_+", "_").Trim('_');
        return string.IsNullOrEmpty(s) ? "medicine" : s;
    }

    private struct Preset
    {
        public string th, en, img, group, mechanism, instruction, caution;
        public Preset(string th, string en, string img, string group, string mechanism, string instruction, string caution)
        {
            this.th = th; this.en = en; this.img = img;
            this.group = group; this.mechanism = mechanism;
            this.instruction = instruction; this.caution = caution;
        }
    }
}
