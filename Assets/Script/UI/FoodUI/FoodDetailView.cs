using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Actual food detail view controller
public class FoodDetailView : MonoBehaviour
{
    [Header("Frame")]
    public GameObject root; // enable/disable this (can be same as this GO)

    [Header("UI")]
    public TMP_Text titleTH;
    public Image photo;
    public TMP_Text giValue;
    public TMP_Text glValue;
    public TMP_Text calories;
    public TMP_Text fat;
    public TMP_Text chol;
    public TMP_Text sodium;
    public TMP_Text carb;
    public TMP_Text protein;

    public void Show(FoodDataSO f)
    {
        if (root) root.SetActive(true);
        if (titleTH) titleTH.text = f.foodNameTH;
        if (photo) photo.sprite = f.foodImage;
        if (giValue) giValue.text = Mathf.RoundToInt(f.glycemicIndex).ToString();
        if (glValue) glValue.text = Mathf.RoundToInt(f.glycemicLoad).ToString();
        if (calories) calories.text = $"~{Mathf.RoundToInt(f.calories)} kcal";
        if (fat) fat.text = $"~{f.totalFat} g";
        if (chol) chol.text = $"~{f.cholesterol} mg";
        if (sodium) sodium.text = $"~{f.sodium} mg";
        if (carb) carb.text = $"~{f.carbohydrate} g";
        if (protein) protein.text = $"~{f.protein} g";
    }

    public void Hide() { if (root) root.SetActive(false); }
}
