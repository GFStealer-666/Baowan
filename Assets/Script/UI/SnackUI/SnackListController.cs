using System.Collections.Generic;
using UnityEngine;

public class SnackListController : MonoBehaviour
{
    [SerializeField] List<SnacksDataSO> items = new();  // assign in Inspector
    [SerializeField] Transform contentParent;           // ScrollView Content / Vertical Group
    [SerializeField] SnackButton buttonPrefab;          // prefab with SnackButton
    [SerializeField] UnifiedDetailView detailView;      // the unified detail panel

    void Start()
    {
        foreach (Transform c in contentParent) Destroy(c.gameObject);
        foreach (var so in items)
        {
            var btn = Instantiate(buttonPrefab, contentParent);
            btn.Setup(so, detailView);
        }
    }
}
