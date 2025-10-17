using UnityEngine;

[CreateAssetMenu(fileName = "NewMedicineData", menuName = "Health/Medicine Data")]
public class MedicineDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string medNameTH;
    public string medNameEN;
    public Sprite image;

    [Header("Drug Group / Class (กลุ่มยา)")]
    public string group;

    [Header("Mechanism (กลไกการออกฤทธิ์)")]
    [TextArea(3, 6)] public string mechanism;

    [Header("Instruction (วิธีรับประทาน)")]
    [TextArea(2, 6)] public string instruction;

    [Header("Caution / Warning (ข้อควรระวัง)")]
    [TextArea(2, 6)] public string caution;
}
