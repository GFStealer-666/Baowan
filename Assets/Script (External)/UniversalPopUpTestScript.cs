/*
 * this is the test script. very easy to use plugin
 * Just make sure UICanvas and EventSystem exist in the scene
 * 
 * */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UniversalPopUpTestScript : MonoBehaviour
{
    [Space]
    [Header("Dialogue Box Objects")]
    public Text txtDialoguePressed;
    public InputField inpFldTitleForInputDialogueBox;
    public InputField inpFldMessageForInputDialogueBox;
    public Dropdown drpDwnMessageBoxMode;
    private UniversalDialogueInputPopUp.MESSAGE_TYPE m_viewMode = UniversalDialogueInputPopUp.MESSAGE_TYPE.NORMAL;

    [Space]
    [Header("Loading Box Objects")]
    public InputField inpFldMessageForLoadingWithMessageAndEllipsis;
    public InputField inpFldLoadingTime;

    [Space]
    [Header("Calendar Box Objects")]
    public Text txtSelectedDate;

    #region dialogue box demo
    public void TestOkMessageBox()
    {
        SetMessageBoxViewMode();
        //you can ommit the last argument in this function call
        UniversalPopUps.ShowOkMessage(inpFldMessageForInputDialogueBox.text, inpFldTitleForInputDialogueBox.text, m_viewMode, OnPressOk);
        //You can also pass array of strings and the messagebox will auto adjust
    }

    public void TestOkCancelMessageBox()
    {
        SetMessageBoxViewMode();
        //you can ommit the last argument in this function call
        UniversalPopUps.ShowOkCancelPopUp(inpFldMessageForInputDialogueBox.text, inpFldTitleForInputDialogueBox.text, m_viewMode, OnPressOk, OnPressCancel);
        //You can also pass array of strings and the messagebox will auto adjust
    }

    void SetMessageBoxViewMode() //process what type of messagebox it would be base on user dropdown input
    {
        switch (drpDwnMessageBoxMode.value) 
        {
            case 0:
            m_viewMode = UniversalDialogueInputPopUp.MESSAGE_TYPE.NORMAL;
            break;
            case 1:
            m_viewMode = UniversalDialogueInputPopUp.MESSAGE_TYPE.WARNING;
            break;
            case 2:
            m_viewMode = UniversalDialogueInputPopUp.MESSAGE_TYPE.ERROR;
            break;
        }
    }

    #region callbacks/hooks ---- this functions are passed as an argument and will be called once the user press a ok/cancel
    void OnPressOk() //if OK is pressed call this function
    {
        txtDialoguePressed.text = "Pressed: Ok";
    }

    void OnPressCancel() //if cancel is pressed call this function
    {
        txtDialoguePressed.text = "Pressed: Cancel";
    }
    #endregion
    #endregion

    #region Loading Demo
    public void TestLoadingWholeScreen() 
    {
        UniversalPopUps.ShowLoading(); //call the loading whole screen
        StartCoroutine(RunLoadingForGivenTime()); //set how long the loading will be displayed and then hide the loading
    }

    public void TestLoadingWithMessageAndEllipsis()
    {
        UniversalPopUps.ShowLoading(inpFldMessageForLoadingWithMessageAndEllipsis.text);//call the loading with messages and ellipsis animation
        StartCoroutine(RunLoadingForGivenTime()); //set how long the loading will be displayed and then hide the loading
    }

    IEnumerator RunLoadingForGivenTime() 
    {
        yield return new WaitForSeconds(float.Parse(inpFldLoadingTime.text));
        UniversalPopUps.HideLoading(); // hide the loading screen
    }
    #endregion

    #region Calendar Picker and Birthday Picker
    public void TestCalendarPicker() 
    {
        //Invoke the calendar picker
        UniversalPopUps.ShowCalendarPopUp(OnDateSelected);
    }

    public void TestBirthdayPicker() 
    {
        //Invoke the Date picker
        UniversalPopUps.ShowBirthDayPickerPopUp(OnDateSelected);
    }

    void OnDateSelected(DateTime p_dateTime) //once a date is selected(in calendar picker) or done is cliked(in date picker) this function will be called
    {
        txtSelectedDate.text = "Selected Date: " + p_dateTime.ToShortMonthName() + " " + p_dateTime.Day.ToString() + " " + p_dateTime.Year.ToString();
    }
	#endregion
}
