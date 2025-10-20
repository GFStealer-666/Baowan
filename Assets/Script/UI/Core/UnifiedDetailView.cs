using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnifiedDetailView : MonoBehaviour
{
    [Header("Root (to show/hide)")]
    [SerializeField] GameObject root;     // optional; defaults to this.gameObject

    [Header("UI")]
    [SerializeField] TMP_Text titleText;
    [SerializeField] Image heroImage;
    [SerializeField] TMP_Text bodyText;
    [SerializeField] TMP_Text tipText;    // optional, visible only when needed

    void Awake()
    {
        if (!root) root = gameObject;
        if (tipText) tipText.gameObject.SetActive(false);
    }

    public void Hide() => root.SetActive(false);

    // ---------------------- Main Dish ----------------------
    public void Show(MainDishDataSO so)
    {
        root.SetActive(true);
        SetHeader(so.dishNameTH, so.dishNameEN, so.image);

        var sb = new StringBuilder();
        Append(sb, "<b>ชื่อภาษาอังกฤษ</b>", so.dishNameEN);
        Append(sb, "<b>วัตถุดิบ</b>", so.ingredients);
        Append(sb, "<b>วิธีทำ</b>", so.method);
        if (so.calories > 0) Append(sb, "<b>พลังงาน</b>", $"{so.calories} kcal");
        bodyText.text = sb.ToString();

        ToggleTip(so.tip);
    }

    // ---------------------- Snack ----------------------
    public void Show(SnacksDataSO so)
    {
        root.SetActive(true);
        SetHeader(so.snackNameTH, so.snackNameEN, so.image);

        var sb = new StringBuilder();
        Append(sb, "<b>ชื่อภาษาอังกฤษ</b>", so.snackNameEN);
        Append(sb, "<b>วัตถุดิบ</b>", so.ingredient);
        Append(sb, "<b>วิธีทำ/คำแนะนำ</b>", so.method);
        if (so.calories > 0) Append(sb, "<b>พลังงาน</b>", $"{so.calories} kcal");
        bodyText.text = sb.ToString();

        ToggleTip(so.tip);
    }

    // ---------------------- Medicine ----------------------
    public void Show(MedicineDataSO so)
    {
        root.SetActive(true);
        SetHeader(so.medNameTH, so.medNameEN, so.image);

        var sb = new StringBuilder();
        Append(sb, "<b>ชื่อภาษาอังกฤษ</b>", so.medNameEN);
        Append(sb, "<b>กลุ่มยา</b>", so.group);
        Append(sb, "<b>กลไกการออกฤทธิ์</b>", so.mechanism);
        Append(sb, "<b>วิธีรับประทาน</b>", so.instruction);
        Append(sb, "<b>ข้อควรระวัง</b>", so.caution);
        bodyText.text = sb.ToString();

        if (tipText) tipText.gameObject.SetActive(false);
    }

    // ---------------------- Drink ----------------------
    public void Show(DrinkDataSO so)
    {
        root.SetActive(true);
        SetHeader(so.drinkNameTH, so.drinkNameEN, so.drinkImage);

        var sb = new StringBuilder();
        Append(sb, "<b>ชื่อภาษาอังกฤษ</b>", so.drinkNameEN);
        Append(sb, "<b>วัตถุดิบ</b>", so.ingredient);
        Append(sb, "<b>วิธีทำ</b>", so.method);

        // Optional nutrition
        if (so.calories > 0) Append(sb, "<b>พลังงาน</b>", $"{so.calories} kcal");
        if (so.glycemicIndex > 0) Append(sb, "<b>Glycemic Index (GI)</b>", so.glycemicIndex.ToString("F1"));
        if (so.glycemicLoad > 0) Append(sb, "<b>Glycemic Load (GL)</b>", so.glycemicLoad.ToString("F1"));

        bodyText.text = sb.ToString();

        // Benefit = same behavior as tip
        ToggleTip(so.benefit);
    }

    // ---------------------- Helpers ----------------------
    void SetHeader(string nameTH, string nameEN, Sprite img)
    {
        if (titleText)
            titleText.text = string.IsNullOrEmpty(nameTH) ? nameEN : nameTH;
        if (heroImage)
            heroImage.sprite = img;
    }

    void ToggleTip(string text)
    {
        if (!tipText) return;
        if (!string.IsNullOrWhiteSpace(text))
        {
            tipText.text = text;
            tipText.gameObject.SetActive(true);
        }
        else
        {
            tipText.gameObject.SetActive(false);
        }
    }

    static void Append(StringBuilder sb, string header, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return;
        sb.AppendLine(header);
        sb.AppendLine(content.Trim());
        sb.AppendLine();
    }
}
