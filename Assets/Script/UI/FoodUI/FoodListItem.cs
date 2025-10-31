using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodListItem : MonoBehaviour
{
    [Header("Refs")]
    public TMP_Text nameText;
    public TMP_Text kcalText;
    public TMP_Text giBadgeText;
    public TMP_Text glBadgeText;
    public Image giBadgeBg;   // Image behind the GI number
    public Image glBadgeBg;   // Image behind the GL number
    public Button clickButton;

    [Header("Badge Sprites")]
    public Sprite greenSprite;
    public Sprite orangeSprite;
    public Sprite redSprite;

    private FoodDataSO bound;

    public void Bind(FoodDataSO data, System.Action<FoodDataSO> onClick)
    {
        bound = data;

        if (nameText)   nameText.text  = data.foodNameTH;
        if (kcalText)   kcalText.text  = $"{Mathf.RoundToInt(data.calories)} kcal";

        int gi = Mathf.RoundToInt(data.glycemicIndex);
        int gl = Mathf.RoundToInt(data.glycemicLoad);

        if (giBadgeText) giBadgeText.text = gi.ToString();
        if (glBadgeText) glBadgeText.text = gl.ToString();

        // Set sprites by rule
        if (giBadgeBg) giBadgeBg.sprite = GetGISprite(gi);
        if (glBadgeBg) glBadgeBg.sprite = GetGLSprite(gl);

        // (Optional) adjust text color for contrast on red
        if (giBadgeText) giBadgeText.color = (gi >= 70) ? Color.white : Color.black;
        if (glBadgeText) glBadgeText.color = (gl >= 20) ? Color.white : Color.black;

        clickButton.onClick.RemoveAllListeners();
        clickButton.onClick.AddListener(() => onClick?.Invoke(bound));
    }

    private Sprite GetGISprite(int gi)
    {
        if (gi <= 55) return greenSprite;     // 0–55
        if (gi <= 69) return orangeSprite;    // 56–69
        return redSprite;                      // 70+
    }

    private Sprite GetGLSprite(int gl)
    {
        if (gl <= 10) return greenSprite;     // 0–10
        if (gl <= 19) return orangeSprite;    // 11–19
        return redSprite;                      // 20+
    }

    public FoodDataSO Data => bound;
}
