/* 
 * 
 * 
 * Universal Pop Ups (new)
 * 
 * Make Sure Canvas is existing in your scene then thats it. you can call the functions here
 * 
 * */

using System;
using UnityEngine;

public static class UniversalPopUps
{
	#region Loading Popups
	public static void ShowLoading() 
    {
        CheckLoadingInstance();
        UniversalLoading.Instance.ShowLoading();
    }

    public static void ShowLoading(string p_messge)
    {
        CheckLoadingInstance();
        UniversalLoading.Instance.ShowLoading(p_messge);
    }

    public static void HideLoading()
    {
        CheckLoadingInstance();
        UniversalLoading.Instance.HideLoading();
    }

    private static void CheckLoadingInstance() 
    {
        if (UniversalLoading.Instance == null)
        {
            GameObject.Instantiate(Resources.Load("UniversolPopUps/UniversalLoading"));
        }
    }
    #endregion

    #region dialogue input 
    public static void ShowOkCancelPopUp(string p_message, string p_title, UniversalDialogueInputPopUp.MESSAGE_TYPE p_messageType, Action p_yesAction, Action p_cancelAction = null)
    {
        CheckDialogueInputInstance();
        UniversalDialogueInputPopUp.Instance.ShowOkCancelPopUp(p_message, p_title, p_messageType, p_yesAction, p_cancelAction);
    }

    public static void ShowOkCancelPopUp(string[] p_messages, string p_title, UniversalDialogueInputPopUp.MESSAGE_TYPE p_messageType, Action p_yesAction, Action p_cancelAction = null)
    {
        CheckDialogueInputInstance();
        UniversalDialogueInputPopUp.Instance.ShowOkCancelPopUp(p_messages, p_title, p_messageType, p_yesAction, p_cancelAction);
    }

    public static void ShowOkMessage(string p_message, string p_title, UniversalDialogueInputPopUp.MESSAGE_TYPE p_messageType, Action p_okAction = null)
    {
        CheckDialogueInputInstance();
        UniversalDialogueInputPopUp.Instance.ShowOkMessage(p_message, p_title, p_messageType, p_okAction);
    }

    public static void ShowOkMessage(string[] p_messages, string p_title, UniversalDialogueInputPopUp.MESSAGE_TYPE p_messageType, Action p_okAction = null)
    {
        CheckDialogueInputInstance();
        UniversalDialogueInputPopUp.Instance.ShowOkMessage(p_messages, p_title, p_messageType, p_okAction);
    }

    private static void CheckDialogueInputInstance()
    {
        if (UniversalDialogueInputPopUp.Instance == null)
        {
            GameObject.Instantiate(Resources.Load("UniversolPopUps/UniversalDialogueInputPopUp"));
        }
    }
    #endregion

    #region Calendar Popups
    public static void ShowCalendarPopUp(Action<DateTime> p_clientAction)
    {
        CheckCalendarPopUpInstance();
        UniversalCalendarPopUp.Instance.ShowCalendarPopUp(p_clientAction);
    }

    public static void ShowBirthDayPickerPopUp(Action<DateTime> p_clientAction)
    {
        CheckCalendarPopUpInstance();
        UniversalCalendarPopUp.Instance.ShowBirthDayPickerPopUp(p_clientAction);
    }

    private static void CheckCalendarPopUpInstance()
    {
        if (UniversalCalendarPopUp.Instance == null)
        {
            GameObject.Instantiate(Resources.Load("UniversolPopUps/UniversalCalendarPopUp"));
        }
    }
    #endregion
}
