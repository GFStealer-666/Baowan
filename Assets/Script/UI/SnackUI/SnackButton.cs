using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnackButton : MonoBehaviour
{
    [SerializeField] TMP_Text nameLabel;
    SnacksDataSO data;
    UnifiedDetailView detail;

    public void Setup(SnacksDataSO so, UnifiedDetailView view)
    {
        data = so; detail = view;
        if (nameLabel) nameLabel.text = string.IsNullOrEmpty(so.snackNameTH) ? so.snackNameEN : so.snackNameTH;
    }

    public void OnClick()
    {
        if (detail && data) detail.Show(data);
    }
}
