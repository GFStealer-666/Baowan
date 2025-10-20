using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainDishButton : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_Text nameLabel;

    MainDishDataSO data;
    [SerializeField] UnifiedDetailView detailView;

    public void Setup(MainDishDataSO so, UnifiedDetailView view)
    {
        data = so;
        detailView = view;

        if (nameLabel)
            nameLabel.text = string.IsNullOrEmpty(so.dishNameTH) ? so.dishNameEN : so.dishNameTH;
    }

    // Hook this to the Button.onClick() in prefab
    public void OnClick()
    {
        if (detailView && data)
            detailView.Show(data);
    }
}
