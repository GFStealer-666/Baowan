using System.Collections.Generic;
using UnityEngine;

public class MainDishListController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] List<MainDishDataSO> dishes = new();  // assign in Inspector

    [Header("UI")]
    [SerializeField] Transform contentParent;              // your VerticalLayout/ScrollView Content
    [SerializeField] MainDishButton buttonPrefab;          // the prefab with MainDishButton

    [Header("Detail")]
    [SerializeField] UnifiedDetailView detailView;        // reference to the detail screen

    void Start() => BuildList();

    void BuildList()
    {
        foreach (Transform c in contentParent)
            Destroy(c.gameObject);

        foreach (var so in dishes)
        {
            var btn = Instantiate(buttonPrefab, contentParent);
            btn.Setup(so, detailView);
        }
    }
}
