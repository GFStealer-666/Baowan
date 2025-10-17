using UnityEngine;

public class FoodDisplayManager : MonoBehaviour
{
    public FoodPanelUI panelUI;
    public FoodDataSO currentFood;

    void Start()
    {
        panelUI.Setup(currentFood);
    }
}
