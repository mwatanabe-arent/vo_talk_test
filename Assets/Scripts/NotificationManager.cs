using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class NotificationManager : MonoBehaviour
{
    private void Start()
    {
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new System.TimeSpan(0, 0, 10),
            Repeats = false,
        };

        var notification = new iOSNotification()
        {
            Identifier = "latest_news",
            Title = "新しいニュース情報",
            Body = "話題のニュースが更新されたよ！",
            Subtitle = "サブタイトル",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };
        //iOSNotificationCenter.RemoveAllScheduledNotifications();
        iOSNotificationCenter.ScheduleNotification(notification);
    }
}
