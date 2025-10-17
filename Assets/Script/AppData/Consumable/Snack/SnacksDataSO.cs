using UnityEngine;

[CreateAssetMenu(fileName = "NewSnackData", menuName = "Food/Snack Data")]
public class SnacksDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string snackNameTH;
    public string snackNameEN;
    public Sprite image;

    [Header("Ingredients (วัตถุดิบ)")]
    [TextArea(2, 5)] public string ingredient;

    [Header("Method (วิธีทำหรือคำแนะนำ)")]
    [TextArea(2, 5)] public string method;

    [Header("Calories (optional)")]
    public float calories;

    [Header("Tips / Benefits")]
    [TextArea(2, 6)] public string tip;
}
