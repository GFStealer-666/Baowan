using System.Collections.Generic;
using UnityEngine;

public class MedicineListController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] List<MedicineDataSO> items = new();   // assign in Inspector

    [Header("UI")]
    [SerializeField] Transform contentParent;              // your ButtonParent / ScrollView Content
    [SerializeField] MedicineButton buttonPrefab;          // the prefab with MedicineButton

    [Header("Detail")]
    [SerializeField] MedicineDetailView detailView;        // reference to the detail view

    void Start() => BuildList();

    public void BuildList()
    {
        // clear old
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        // spawn
        foreach (var so in items)
        {
            var btn = Instantiate(buttonPrefab, contentParent);
            btn.Setup(so, detailView);
        }
    }
}
    