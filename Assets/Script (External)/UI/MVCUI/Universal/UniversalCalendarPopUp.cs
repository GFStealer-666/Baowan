using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniversalCalendarPopUp : MonoBehaviour { 
    
    private const int NUMBER_CALENDAR_DAYS = 42;

    public static UniversalCalendarPopUp Instance = null;

    public GameObject btnPerDayToInstantiate;
    public Transform buttonParents;

    private DateTime m_targetDay = DateTime.Now;
    private List<UniversalCalendarDay> m_btnDays = new List<UniversalCalendarDay>();

    public GameObject parentDisplay;
    public GameObject calendarPickerParent;
    public GameObject birthdayPickerParent;

    [Space]
    [Header("Calendar Picker Assets")]
    public Text txtMonthAndYear;
    public Button btnNextMonth;
    public Button btnPreviousMonth;

    [Space]
    [Header("Birthday Picker Assets")]
    public Dropdown drpDownDay;
    public Dropdown drpDownMonth;
    public Dropdown drpDownYear;

    public Button btnDone;
    public Button btnCancel;

    private Action<DateTime> m_clientAction;
    private bool _isInitialized;

	private void OnEnable()
	{
        if (Instance == null) {
            Instance = this;
        }
        btnNextMonth.onClick.AddListener(DisplayNextMonth);
        btnPreviousMonth.onClick.AddListener(DisplayPreviousMonth);
        btnDone.onClick.AddListener(ClickDone);
        btnCancel.onClick.AddListener(ClickCancel);
        UniversalCalendarDay.onDayPressed += OnPressButton;
    }

	private void OnDisable()
	{
        if (Instance == this)
        {
            Instance = null;
        }
        btnNextMonth.onClick.RemoveListener(DisplayNextMonth);
        btnPreviousMonth.onClick.RemoveListener(DisplayPreviousMonth);
        btnDone.onClick.RemoveListener(ClickDone);
        btnCancel.onClick.RemoveListener(ClickCancel);
        UniversalCalendarDay.onDayPressed -= OnPressButton;
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

    public void ShowCalendarPopUp(Action<DateTime> p_clientAction)
	{
        m_targetDay = DateTime.Now;
        ShowCalendarPopUp();
        m_clientAction = p_clientAction;
        InitUI();
        if (!_isInitialized)
        {
            _isInitialized = true;
            SpawnDaysItem();
        }
        InitializeCalendarDisplay(m_targetDay.Year, m_targetDay.Month);
        UpdateMonthAndYearDisplay();
    }

    public void ShowBirthDayPickerPopUp(Action<DateTime> p_clientAction)
    {
        m_targetDay = DateTime.Now;
        ShowBirthDayPickerPopUp();
        m_clientAction = p_clientAction;
        InitUI();
    }

    void InitializeCalendarDisplay(int p_year, int p_month) 
    {
        DateTime dateTime = new DateTime(p_year, p_month, 1);
        List<CalendarTool.CalendarDisplayData> monthDate = CalendarTool.GetNumberDaysOfAMonth(dateTime.Year, dateTime.Month, NUMBER_CALENDAR_DAYS);

        for (int x = 0; x < monthDate.Count; ++x)
        {
            m_btnDays[x].InitButton(monthDate[x].dateTime, monthDate[x].day.ToString(), monthDate[x].isPartOfMonth, monthDate[x].isPartOfMonth ? new Color(0f, 0f, 0f, 1f) : new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }
    }

    void SpawnDaysItem()
    {
        for (int x = 0; x < NUMBER_CALENDAR_DAYS; ++x)
        {
            UniversalCalendarDay btnDays = Instantiate(btnPerDayToInstantiate).GetComponent<UniversalCalendarDay>();
            btnDays.transform.SetParent(buttonParents, false);
            m_btnDays.Add(btnDays);
        }
    }

    void UpdateMonthAndYearDisplay() {
        txtMonthAndYear.text = m_targetDay.ToShortMonthName() + " " + m_targetDay.Year.ToString();
    }

    void DisplayNextMonth() 
    {
        m_targetDay = m_targetDay.AddMonths(1);
        UpdateMonthAndYearDisplay();
        InitializeCalendarDisplay(m_targetDay.Year, m_targetDay.Month);
    }

    void DisplayPreviousMonth() {
        m_targetDay = m_targetDay.AddMonths(-1);
        UpdateMonthAndYearDisplay();
        InitializeCalendarDisplay(m_targetDay.Year, m_targetDay.Month);
    }

    void OnPressButton(DateTime p_dateTimeClicked) 
    {
        m_clientAction?.Invoke(p_dateTimeClicked);
        m_clientAction = null;
        HidePopUps();
    }

    void ShowCalendarPopUp() {
        parentDisplay.SetActive(true);
        calendarPickerParent.SetActive(true);
        birthdayPickerParent.SetActive(false);
    }

    void ShowBirthDayPickerPopUp()
    {
        parentDisplay.SetActive(true);
        calendarPickerParent.SetActive(false);
        birthdayPickerParent.SetActive(true);
    }

    void HidePopUps() 
    {
        parentDisplay.SetActive(false);
    }

    #region button triggers
    void ClickDone() 
    {
        int day = drpDownDay.value + 1;
        int month = drpDownMonth.value + 1;
        int year = 0;
        int.TryParse(drpDownYear.captionText.text, out year);
        m_targetDay = new DateTime(year, month, day);
        m_clientAction?.Invoke(m_targetDay);
        HidePopUps();
    }

    void ClickCancel() 
    {
        HidePopUps();
    }
	#endregion
}