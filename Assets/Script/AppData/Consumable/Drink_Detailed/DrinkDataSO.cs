using UnityEngine;

[CreateAssetMenu(fileName = "NewDrinkData", menuName = "Food/Drink Data")]
public class DrinkDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string drinkNameTH;      // ชื่อภาษาไทย เช่น ชาเขียวไม่หวาน
    public string drinkNameEN;      // English name
    public Sprite drinkImage;       // รูปภาพเครื่องดื่ม

    [Header("Ingredients & Method")]
    [TextArea(2, 5)]
    public string ingredient;       // วัตถุดิบ
    [TextArea(2, 5)]
    public string method;           // วิธีทำ

    [Header("Nutrition Info (optional)")]
    public float calories;
    public float glycemicIndex;
    public float glycemicLoad;

    [Header("Additional Info")]
    [TextArea(2, 5)]
    public string benefit;          // ประโยชน์ เช่น มี Catechins ช่วยลดไขมันในเลือด
}
