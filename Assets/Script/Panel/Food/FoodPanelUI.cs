using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodPanelUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text foodNameTHText;
    public TMP_Text foodNameENText;
    public Image foodImage;
    public TMP_Text glycemicIndexText;
    public TMP_Text glycemicLoadText;
    public TMP_Text caloriesText;
    public TMP_Text fatText;
    public TMP_Text cholesterolText;
    public TMP_Text sodiumText;
    public TMP_Text carbohydrateText;
    public TMP_Text proteinText;

    public void Setup(FoodDataSO data)
    {
        foodNameTHText.text = data.foodNameTH;
        foodNameENText.text = data.foodNameEN;
        foodImage.sprite = data.foodImage;

        glycemicIndexText.text = $"~{data.glycemicIndex}";
        glycemicLoadText.text = $"~{data.glycemicLoad}";

        caloriesText.text = $"~{data.calories} kcal";
        fatText.text = $"~{data.totalFat} g";
        cholesterolText.text = $"~{data.cholesterol} mg";
        sodiumText.text = $"~{data.sodium} mg";
        carbohydrateText.text = $"~{data.carbohydrate} g";
        proteinText.text = $"~{data.protein} g";
    }
}
