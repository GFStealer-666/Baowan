using UnityEngine;

[CreateAssetMenu(fileName = "NewMainDishData", menuName = "Food/Main Dish Data")]
public class MainDishDataSO : ScriptableObject
{
    [Header("Basic")]
    public string dishNameTH;
    public string dishNameEN;
    public Sprite image;

    [Header("Ingredients (วัตถุดิบ)")]
    [TextArea(3, 6)] public string ingredients;

    [Header("Method (วิธีทำ)")]
    [TextArea(3, 8)] public string method;

    [Header("Optional Nutrition")]
    public float calories;

    [Header("Tip / Benefit")]
    [TextArea(2, 6)] public string tip;
}
