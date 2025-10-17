using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UniversalLoading : MonoBehaviour
{
    public static UniversalLoading Instance = null;

    public Transform parentDisplay;
    public Text txtMessage;

    public GameObject wholeScreenLoadingParent;
    public GameObject popUpMessageLoadingParent;

    private string m_originalMessage;
    private bool m_playAnimation = false;
    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void InitUI() 
    {
        if (transform.parent == null || transform.parent.GetComponent<Canvas>() == null)
        {
            transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
            transform.localPosition = Vector3.zero;
        }
        transform.SetAsLastSibling();
    }

    public void ShowLoading()
    { 
        InitUI();
        ShowPopUp();
        wholeScreenLoadingParent.SetActive(true);
        popUpMessageLoadingParent.SetActive(false);
    }

    public void ShowLoading(string p_message)
    {
        InitUI();
        ShowPopUp();
        wholeScreenLoadingParent.SetActive(false);
        popUpMessageLoadingParent.SetActive(true);
        m_originalMessage = p_message;
        txtMessage.text = p_message;
        if (!m_playAnimation)
        {
            StartCoroutine(AnimateEllipsis());
        }
    }

    IEnumerator AnimateEllipsis()
    {
        m_playAnimation = true;
        int ellipsisCount = 0;
        txtMessage.text = m_originalMessage;
        while (m_playAnimation)
        {
            txtMessage.text += ".";
            ellipsisCount++;
            if (ellipsisCount >= 4)
            {
                txtMessage.text = m_originalMessage;
                ellipsisCount = 0;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void ShowPopUp()
    {
        parentDisplay.gameObject.SetActive(true);
    }

    public void HideLoading()
    {
        m_playAnimation = false;
        parentDisplay.gameObject.SetActive(false);
    }
}