using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodListItem : MonoBehaviour
{
    [Header("Refs")]
    public Image thumbnail;
    public TMP_Text nameText;
    public TMP_Text kcalText;
    public TMP_Text giBadgeText;
    public TMP_Text glBadgeText;
    public Button clickButton;

    private FoodDataSO bound;

    public void Bind(FoodDataSO data, System.Action<FoodDataSO> onClick)
    {
        bound = data;
        if (thumbnail)  thumbnail.sprite = data.foodImage;
        if (nameText)   nameText.text  = string.IsNullOrEmpty(data.foodNameEN) ? data.foodNameTH : data.foodNameEN;
        if (kcalText)   kcalText.text  = $"{Mathf.RoundToInt(data.calories)} kcal";
        if (giBadgeText) giBadgeText.text = Mathf.RoundToInt(data.glycemicIndex).ToString();
        if (glBadgeText) glBadgeText.text = Mathf.RoundToInt(data.glycemicLoad).ToString();

        clickButton.onClick.RemoveAllListeners();
        clickButton.onClick.AddListener(() => onClick?.Invoke(bound));
    }

    public FoodDataSO Data => bound;
}
