using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Auth;
using System.Threading.Tasks;

public class ProfileUIController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject profilePanel;
    public Image profileImage;
    public TMP_Text emailText;
    public TMP_Text phoneText;
    public TMP_Text ageText;
    public TMP_Text weightText;
    public TMP_Text heightText;
    public TMP_Text careerText;
    public TMP_Text bloodText;

    public Sprite defaultAvatar;  // assign in inspector if no image yet

    // Called by your Profile button OnClick()
    public async void OnProfileButtonClicked()
    {
        // check auth
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null) { Debug.LogWarning("No user signed in."); return; }

        profilePanel.SetActive(true);
        emailText.text = "Loading...";

        // (optional) ensure Firebase ready here too
        await FirebaseReady.Ensure();

        var p = await UserProfileService.Instance.GetProfile(user.UserId);
        if (p == null) { emailText.text = "Profile not found"; return; }
        profilePanel.SetActive(true);
        // populate fields
        emailText.text = $"{p.email}";
        phoneText.text = $"{p.phone}";
        ageText.text = p.age.HasValue ? $"{p.age} Years Old" : "Age : -";
        weightText.text = p.weightKg.HasValue ? $"{p.weightKg} kg" : "Weight : -";
        heightText.text = p.heightCm.HasValue ? $"{p.heightCm} cm" : "Height : -";
        careerText.text = $"{p.career}";
        bloodText.text = p.bloodGlucoseMgDl.HasValue ? $"{p.bloodGlucoseMgDl} mg/dL" : "Blood Glucose : -";

        // avatar (if you store one later)
        profileImage.sprite = defaultAvatar;
    }

    // Close button
    public void OnCloseProfile()
    {
        profilePanel.SetActive(false);
    }
}
