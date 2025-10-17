using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UniversalCalendarDay : MonoBehaviour
{
	public static Action<DateTime> onDayPressed;
    private Button m_btnDay;
	private Text m_txtDay;
	private DateTime m_timeSlot;

	private void OnEnable()
	{
		m_btnDay.onClick.AddListener(PressDay);
	}

	private void OnDisable()
	{
		m_btnDay.onClick.RemoveListener(PressDay);
	}
	
	private void Awake()
	{
		m_btnDay = GetComponent<Button>();
		m_txtDay = GetComponentInChildren<Text>();
	}

	public void InitButton(DateTime p_newTimeSlot, string p_newText, bool p_isClickable, Color? p_newColor = null) {
		m_btnDay.interactable = p_isClickable;
		m_timeSlot = p_newTimeSlot;
		m_txtDay.text = p_newText;
		m_txtDay.color = p_newColor ?? Color.black;
	}

	void PressDay() {
		Debug.Log(m_timeSlot.Date);
		onDayPressed?.Invoke(m_timeSlot);
	}
}
