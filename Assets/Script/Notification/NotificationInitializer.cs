using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

namespace Baowan.Systems.Notifications
{
    public class NotificationInitializer : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID
            // Create a notification channel
            var channel = new AndroidNotificationChannel
            {
                Id = "baowan_reminder_channel",
                Name = "Baowan Reminders",
                Description = "Medication and other reminders",
                Importance = Importance.High
            };

            AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif

        }
    }
}
