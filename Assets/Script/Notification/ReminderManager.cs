using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

namespace Baowan.Systems.Notifications
{
    public class ReminderManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField timeInputField; // "HH:MM"
        [SerializeField] private Button setReminderButton;

        [Header("Reminder Content")]
        [SerializeField] private string reminderTitle = "แจ้งเตือนทานยา";
        [SerializeField] private string reminderMessage = "ถึงเวลาทานยาแล้ว";

        private void Awake()
        {
            if (setReminderButton != null)
                setReminderButton.onClick.AddListener(OnSetReminderClicked);
        }
        void Start()
        {
            RequestNotificationPermission();
            RegisterNotificationChannel();
        }
        private void OnDestroy()
        {
            if (setReminderButton != null)
                setReminderButton.onClick.RemoveListener(OnSetReminderClicked);
        }
        
        public void RequestNotificationPermission()
        {
            if(!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }
        }
        
        private void OnSetReminderClicked()
        {
            if (timeInputField == null)
            {
                Debug.LogError("ReminderManager: TimeInputField is not assigned.");
                return;
            }

            string input = timeInputField.text.Trim(); // e.g. "14:30"

            if (!TryParseTime(input, out DateTime targetTime))
            {
                Debug.LogError($"ReminderManager: Cannot parse time '{input}'. Expected format HH:MM (24h).");
                return;
            }

            ScheduleNotification(targetTime);
        }

        /// <summary>
        /// Parse "HH:MM" into a DateTime (today or tomorrow if time already passed).
        /// </summary>
        private bool TryParseTime(string input, out DateTime result)
        {
            result = DateTime.Now;

            if (string.IsNullOrEmpty(input))
                return false;

            string[] parts = input.Split(':');
            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0], out int hour)) return false;
            if (!int.TryParse(parts[1], out int minute)) return false;

            if (hour < 0 || hour > 23) return false;
            if (minute < 0 || minute > 59) return false;

            DateTime now = DateTime.Now;
            DateTime fireTime = new DateTime(
                now.Year, now.Month, now.Day,
                hour, minute, 0
            );

            // If time already passed today, schedule for tomorrow
            if (fireTime <= now)
                fireTime = fireTime.AddDays(1);

            result = fireTime;
            return true;
        }

        private void ScheduleNotification(DateTime fireTime)
        {
#if UNITY_ANDROID
            ScheduleAndroidNotification(fireTime);
#else
            Debug.Log($"[Editor] Notification scheduled for: {fireTime}");
#endif
        }

#if UNITY_ANDROID
        public void RegisterNotificationChannel()
        {
            var chanel = new AndroidNotificationChannel()
            {
                Id = "baowan_reminder_channel",
                Name = "Baowan Reminders",
                Importance = Importance.High,
                Description = "แจ้งเตือนการทานยา",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(chanel);
        }
        private void ScheduleAndroidNotification(DateTime fireTime)
        {
            var notification = new AndroidNotification
            {
                Title = reminderTitle,
                Text = reminderMessage,
                SmallIcon = "default",  // make sure you have an icon set in Notification Settings
                FireTime = fireTime,
                ShouldAutoCancel = true
            };

            AndroidNotificationCenter.SendNotification(notification, "baowan_reminder_channel");

            Debug.Log($"Android notification scheduled at {fireTime}");
        }
#endif

    }
}

