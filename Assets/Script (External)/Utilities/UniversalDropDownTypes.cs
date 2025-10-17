using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Globalization;

namespace UIUtil {
    [RequireComponent(typeof(Dropdown))]
    public class UniversalDropDownTypes : MonoBehaviour
    {
		public enum TIME_TYPE { MONTH = 0, DAYS, HOUR, MINUTES, SECONDS, MERIDIEMS, LANGUAGE, TIMEZONE, MEASUREMENT, GENDER, AGE, CIVIL_STATUS, YEARS }
		private Dropdown m_dropDown;

		public TIME_TYPE timeType = TIME_TYPE.MONTH;
		private void Awake()
		{
			m_dropDown = GetComponent<Dropdown>();
		}

		private void Start()
		{
			switch (timeType) {
				case TIME_TYPE.MONTH:	
				DoMonth();
				break;
				case TIME_TYPE.DAYS:
				DoDays();
				break;
				case TIME_TYPE.HOUR:
				DoHours();
				break;
				case TIME_TYPE.MINUTES:
				case TIME_TYPE.SECONDS:
				DoSecondsOrMinutes();
				break;
				case TIME_TYPE.MERIDIEMS:
				DoMeridiems();
				break;
				case TIME_TYPE.LANGUAGE:
				DoLanguage();
				break;
				case TIME_TYPE.TIMEZONE:
				DoTimeZone();
				break;
				case TIME_TYPE.MEASUREMENT:
				DoMeasures();
				break;
				case TIME_TYPE.GENDER:
				DoGender();
				break;
				case TIME_TYPE.AGE:
				DoAge();
				break;
				case TIME_TYPE.CIVIL_STATUS:
				DoCivilStatus();
				break;
				case TIME_TYPE.YEARS:
				DoYears();
				break;
			}
		}

		void DoMonth() {
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			for (int x = 0; x < 12; ++x) {
				string monthName = new DateTime(2010, x + 1, 1).ToString("MMM", CultureInfo.InvariantCulture);
				options.Add(new Dropdown.OptionData(monthName));
			}
			m_dropDown.AddOptions(options);
		}

		void DoAge()
		{
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			for (int x = 13; x < 90; ++x)
			{
				options.Add(new Dropdown.OptionData(x.ToString()));
			}
			m_dropDown.AddOptions(options);
		}

		void DoCivilStatus()
		{
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			options.Add(new Dropdown.OptionData("Married"));
			options.Add(new Dropdown.OptionData("Single"));
			m_dropDown.AddOptions(options);
		}

		void DoDays()
		{
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			for (int x = 0; x < 31; ++x) {
				options.Add(new Dropdown.OptionData((x + 1).ToString()));
			}
			m_dropDown.AddOptions(options);
		}

		void DoSecondsOrMinutes()
		{
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			for (int x = 0; x < 60; ++x)
			{
				if (x + 1 < 10)
				{
					options.Add(new Dropdown.OptionData("0" + (x).ToString()));
				} else {
					options.Add(new Dropdown.OptionData((x).ToString()));
				}
				
			}
			m_dropDown.AddOptions(options);
		}

		void DoHours()
		{
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			for (int x = 0; x < 12; ++x)
			{
				options.Add(new Dropdown.OptionData((x + 1).ToString()));
			}
			m_dropDown.AddOptions(options);
		}

		void DoMeridiems() {
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			options.Add(new Dropdown.OptionData("AM"));
			options.Add(new Dropdown.OptionData("PM"));
			m_dropDown.AddOptions(options);
		}

		void DoLanguage()
		{
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			options.Add(new Dropdown.OptionData("China"));
			options.Add(new Dropdown.OptionData("English"));
			options.Add(new Dropdown.OptionData("Korean"));
			options.Add(new Dropdown.OptionData("Thai"));
			options.Add(new Dropdown.OptionData("Malay"));
			m_dropDown.AddOptions(options);
		}

		void DoTimeZone()
		{
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			options.Add(new Dropdown.OptionData("UTC-11"));
			options.Add(new Dropdown.OptionData("UTC+10"));
			options.Add(new Dropdown.OptionData("EST"));
			options.Add(new Dropdown.OptionData("CST"));
			options.Add(new Dropdown.OptionData("MST"));
			m_dropDown.AddOptions(options);
		}

		void DoMeasures()
		{
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			options.Add(new Dropdown.OptionData("Metric"));
			options.Add(new Dropdown.OptionData("English"));
			m_dropDown.AddOptions(options);
		}

		void DoGender()
		{
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			options.Add(new Dropdown.OptionData("Male"));
			options.Add(new Dropdown.OptionData("Female"));
			m_dropDown.AddOptions(options);
		}
		void DoYears() 
		{
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			for (int x = 1940; x <= DateTime.Now.Year; ++x)
			{
				options.Add(new Dropdown.OptionData((x).ToString()));
			}
			m_dropDown.AddOptions(options);
		}
	}
}