using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MedicineButton : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_Text nameLabel;
    [SerializeField] Image icon;     // optional

    MedicineDataSO data;
    [SerializeField]UnifiedDetailView detail;

    public void Setup(MedicineDataSO so, UnifiedDetailView detailView)
    {
        data   = so;
        detail = detailView;

        if (nameLabel) nameLabel.text = string.IsNullOrEmpty(so.medNameTH) ? so.medNameEN : so.medNameTH;
        if (icon)      icon.sprite    = so.image;
    }

    // Hook this to the Button.onClick() in the prefab
    public void OnClick()
    {
        if (detail && data)
        {
            detail.Show(data);
            return;
        }
    }
}
