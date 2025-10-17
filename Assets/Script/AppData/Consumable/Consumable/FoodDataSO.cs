using UnityEngine;

[CreateAssetMenu(fileName = "NewFoodData", menuName = "Food/Food Data")]
public class FoodDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string foodNameTH;
    public string foodNameEN;
    public ConsumeType consumeType;
    public Sprite foodImage;

    [Header("Glycemic Impact")]
    public float glycemicIndex;
    public float glycemicLoad;

    [Header("Nutrition")]
    public float calories;
    public float totalFat;
    public float cholesterol;
    public float sodium;
    public float carbohydrate;
    public float protein;
    
}

public enum ConsumeType
{
    Rice,
    Noodle,
    Beverage,
    Fruit,
    Desert,
    SideDish
}