using System.Collections.Generic;
using UnityEngine;

public class DrinkListController : MonoBehaviour
{
    [SerializeField] List<DrinkDataSO> items = new();
    [SerializeField] Transform contentParent;
    [SerializeField] DrinkButton buttonPrefab;
    [SerializeField] UnifiedDetailView detailView;

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
