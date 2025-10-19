using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoodDatabase : MonoBehaviour
{
    public static FoodDatabase I { get; private set; }

    [Tooltip("Optional: Prewarm sprites in memory (recommended for mobile).")]
    public bool prewarmSprites = true;

    public IReadOnlyList<FoodDataSO> All => _all;
    [SerializeField] private List<FoodDataSO> _all = new List<FoodDataSO>();

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        LoadAll();
    }

    public void LoadAll()
    {
    // Load all FoodDataSO assets placed under any Resources/ScriptableObject subfolder
    // (e.g. Assets/Resources/ScriptableObject/Drink, /Fruit_Veg, /Food)
        _all = Resources.LoadAll<FoodDataSO>("ScriptableObject").OrderBy(f => f.foodNameTH).ToList();

        if (prewarmSprites)
            foreach (var f in _all)
                if (f.foodImage) _ = f.foodImage.texture;
#if UNITY_EDITOR
        Debug.Log($"[FoodDatabase] Loaded {_all.Count} foods.");
#endif
    }

    public IEnumerable<FoodDataSO> RandomPick(int count) =>
        _all.OrderBy(_ => Random.value).Take(Mathf.Min(count, _all.Count));

    public IEnumerable<FoodDataSO> Search(string query, ConsumeType? filter)
    {
        IEnumerable<FoodDataSO> src = _all;
        if (filter.HasValue)
            src = src.Where(f => f.consumeType == filter.Value);

        if (!string.IsNullOrWhiteSpace(query))
        {
            query = query.Trim().ToLowerInvariant();
            src = src.Where(f =>
                (f.foodNameTH != null && f.foodNameTH.ToLowerInvariant().Contains(query)) ||
                (f.foodNameEN != null && f.foodNameEN.ToLowerInvariant().Contains(query)));
        }
        return src;
    }
}
