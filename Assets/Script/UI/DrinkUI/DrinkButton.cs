using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrinkButton : MonoBehaviour
{
    [SerializeField] TMP_Text nameLabel;

    DrinkDataSO data;
    UnifiedDetailView detail;

    public void Setup(DrinkDataSO so, UnifiedDetailView view)
    {
        data = so;
        detail = view;
        if (nameLabel) nameLabel.text = string.IsNullOrEmpty(so.drinkNameTH) ? so.drinkNameEN : so.drinkNameTH;
    }

    public void OnClick()
    {
        if (detail && data) detail.Show(data);
    }
}
