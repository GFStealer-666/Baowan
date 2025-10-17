using System;
using UnityEngine;
using UnityEngine.UI;

public class UniversalDialogueInputPopUp : MonoBehaviour
{
    public Action onOkClicked;
    public Action onCancelClicked;
    public static UniversalDialogueInputPopUp Instance = null;

    public Transform parentDisplay;
    public Button btnOk;
    public Button btnCancel;
    public Text txtMessage;
    public Text txtTitle;
    
    public GameObject errorImage;
    public GameObject warningImage;

    public enum MESSAGE_TYPE { NORMAL = 0, WARNING, ERROR }

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        btnOk.onClick.AddListener(ClickYes);
        btnCancel.onClick.AddListener(ClickCancel);
    }

    private void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        btnOk.onClick.RemoveListener(ClickYes);
        btnCancel.onClick.RemoveListener(ClickCancel);
    }

    void InitSetUp(Action p_yesAction = null, Action p_cancelAction = null)
    {
        if (transform.parent == null || transform.parent.GetComponent<Canvas>() == null)
        {
            transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
            transform.localPosition = Vector3.zero;
        }
        transform.SetAsLastSibling();
        ShowPopUp();
        onOkClicked = p_yesAction;
        onCancelClicked = p_cancelAction;
    }

    public void ShowOkCancelPopUp(string p_message, string p_title, MESSAGE_TYPE p_messageType, Action p_yesAction, Action p_cancelAction = null)
    {
        InitSetUp(p_yesAction, p_cancelAction);
        SetMessage(p_message, p_title, p_messageType);
        btnCancel.gameObject.SetActive(true);
    }

    public void ShowOkCancelPopUp(string[] p_messages, string p_title, MESSAGE_TYPE p_messageType, Action p_yesAction, Action p_cancelAction = null)
    {
        InitSetUp(p_yesAction, p_cancelAction);
        SetMessage(p_messages, p_title, p_messageType);
        btnCancel.gameObject.SetActive(true);
    }

    public void ShowOkMessage(string p_message, string p_title, MESSAGE_TYPE p_messageType, Action p_okAction = null)
    {
        InitSetUp(p_okAction);
        SetMessage(p_message, p_title, p_messageType);
        btnCancel.gameObject.SetActive(false);
    }

    public void ShowOkMessage(string[] p_messages, string p_title, MESSAGE_TYPE p_messageType, Action p_okAction = null)
    {
        InitSetUp(p_okAction);
        SetMessage(p_messages, p_title, p_messageType);
        btnCancel.gameObject.SetActive(false);
    }

    void SetMessage(string[] p_messages, string p_title, MESSAGE_TYPE p_messageType) 
    {
        txtTitle.text = p_title;
        txtMessage.text = String.Empty;
        for (int x = 0; x < p_messages.Length; ++x)
        {
            txtMessage.text += p_messages[x];
            txtMessage.text += "\n";
        }
        SetMessageType(p_messageType);
    }

    void SetMessage(string p_message, string p_title, MESSAGE_TYPE p_messageType)
    {
        txtTitle.text = p_title;
        txtMessage.text = p_message;
        SetMessageType(p_messageType);
    }

    void SetMessageType(MESSAGE_TYPE p_messageType) 
    {
        switch (p_messageType) 
        {
            case MESSAGE_TYPE.NORMAL:   
            errorImage.SetActive(false);
            warningImage.SetActive(false);
            txtTitle.color = Color.black;
            break;
            case MESSAGE_TYPE.WARNING:
            errorImage.SetActive(false);
            warningImage.SetActive(true);
            txtTitle.color = Color.yellow;
            break;
            case MESSAGE_TYPE.ERROR:
            errorImage.SetActive(true);
            warningImage.SetActive(false);
            txtTitle.color = Color.red;
            break;
        }
    }

    void ClickYes()
    {
        onOkClicked?.Invoke();
        HidePopUp();
    }

    void ClickCancel()
    {
        onCancelClicked?.Invoke();
        HidePopUp();
    }

    void ShowPopUp()
    {
        parentDisplay.gameObject.SetActive(true);
    }

    void HidePopUp()
    {
        transform.parent = null;
        onCancelClicked = null;
        onOkClicked = null;
        parentDisplay.gameObject.SetActive(false);
    }
}