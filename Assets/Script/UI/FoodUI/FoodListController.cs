using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodListController : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField searchInput;
    public Transform listContent;
    public GameObject listItemPrefab;
    public FoodDetailView detailView;

    [Header("Filter Buttons (optional)")]
    public Toggle riceToggle;
    public Toggle noodleToggle;
    public Toggle beverageToggle;
    public Toggle fruitToggle;

    [Header("Filter Group")]
    public ToggleGroup filterGroup;   // ← assign in Inspector

    [Header("Defaults")]
    public int defaultRandomCount = 10;

    private readonly List<GameObject> _spawned = new();
    private ConsumeType? _activeFilter = null;

    void Start()
    {
        // Setup ToggleGroup (safety)
        if (filterGroup != null)
        {
            filterGroup.allowSwitchOff = true; // allow "no filter" state

            foreach (var t in new[] { riceToggle, noodleToggle, beverageToggle, fruitToggle })
            {
                if (t == null) continue;
                t.group = filterGroup;
            }
        }

        // Default results: random 10
        RenderItems(FoodDatabase.I.RandomPick(defaultRandomCount));

        // Wire search & filters
        if (searchInput) searchInput.onValueChanged.AddListener(_ => Refresh());
        if (riceToggle)     riceToggle.onValueChanged.AddListener(_ => OnToggleChanged());
        if (noodleToggle)   noodleToggle.onValueChanged.AddListener(_ => OnToggleChanged());
        if (beverageToggle) beverageToggle.onValueChanged.AddListener(_ => OnToggleChanged());
        if (fruitToggle)    fruitToggle.onValueChanged.AddListener(_ => OnToggleChanged());
    }

    // Instead of turning everything ON, let's use this as "clear all filters"
    public void ClearFilter()
    {
        if (filterGroup != null)
            filterGroup.SetAllTogglesOff();   // all off → _activeFilter will be null

        _activeFilter = null;
        Refresh();
    }

    void OnToggleChanged()
    {
        _activeFilter = null;

        if (riceToggle && riceToggle.isOn)           _activeFilter = ConsumeType.Rice;
        else if (noodleToggle && noodleToggle.isOn)  _activeFilter = ConsumeType.Noodle;
        else if (beverageToggle && beverageToggle.isOn) _activeFilter = ConsumeType.Beverage;
        else if (fruitToggle && fruitToggle.isOn)    _activeFilter = ConsumeType.Fruit;

        Refresh();
    }

    void Refresh()
    {
        string q = searchInput ? searchInput.text : "";
        IEnumerable<FoodDataSO> result = FoodDatabase.I.Search(q, _activeFilter);

        // If no query and no filter → show random 10 again
        if (string.IsNullOrWhiteSpace(q) && !_activeFilter.HasValue)
            result = FoodDatabase.I.RandomPick(defaultRandomCount);

        RenderItems(result);
    }

    void RenderItems(IEnumerable<FoodDataSO> items)
    {
        foreach (var go in _spawned) Destroy(go);
        _spawned.Clear();

        foreach (var f in items)
        {
            var go = Instantiate(listItemPrefab, listContent);
            _spawned.Add(go);
            var item = go.GetComponent<FoodListItem>();
            item.Bind(f, OnRowClicked);
        }
    }

    void OnRowClicked(FoodDataSO f)
    {
        if (detailView) detailView.Show(f);
    }
}
