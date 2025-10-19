using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MedicineDetailView : MonoBehaviour
{
    [Header("Root (to show/hide)")]
    [SerializeField] GameObject root;  // set to the detail panel; or leave null to use this.gameObject

    [Header("Header")]
    [SerializeField] TMP_Text titleTH;
    [SerializeField] Image headerImage;

    [Header("Body")]
    [SerializeField] TMP_Text groupText;


    public void Show(MedicineDataSO so)
    {
        if (!root) root = gameObject;
        root.SetActive(true);

        if (titleTH)      titleTH.text      = so.medNameTH;
        if (headerImage)  headerImage.sprite = so.image;

        if (groupText)
        {
            groupText.richText = true;
            groupText.text = BuildDetailText(so);
        }
    }

    private string BuildDetailText(MedicineDataSO so)
    {
        // Helper to safely join sections and preserve spacing/newlines
        string s(string v) => string.IsNullOrWhiteSpace(v) ? string.Empty : v.Trim();

        var parts = new System.Text.StringBuilder();

        // Group (single-line)
        var grp = s(so.group);
        if (!string.IsNullOrEmpty(grp))
        {
            parts.AppendFormat("<b>กลุ่มยา:</b> {0}\n\n", grp);
        }

        // Mechanism (possibly multi-line)
        var mech = s(so.mechanism);
        if (!string.IsNullOrEmpty(mech))
        {
            parts.AppendFormat("<b>การทำงาน:</b>\n{0}\n\n", mech);
        }

        // Instruction
        var instr = s(so.instruction);
        if (!string.IsNullOrEmpty(instr))
        {
            parts.AppendFormat("<b>วิธีการรับประทาน:</b>\n{0}\n\n", instr);
        }

        // Benefits / Good
        // Some data may include multiple lines; keep them
        // We'll label it as ข้อดี (advantages)
        var good = s(so.caution == null ? string.Empty : string.Empty); // placeholder if you separate fields later
        // If your SO has a specific 'good' field, map it here. For now we rely on caution field for warnings.

        // Caution / Warning
        var caution = s(so.caution);
        if (!string.IsNullOrEmpty(caution))
        {
            parts.AppendFormat("<b>ข้อควรระวัง:</b>\n{0}\n", caution);
        }

        return parts.ToString().TrimEnd();
    }

    public void Hide()
    {
        if (!root) root = gameObject;
        root.SetActive(false);
    }
}
