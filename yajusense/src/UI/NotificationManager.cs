using System.Collections.Generic;
using UnityEngine;
using yajusense.Modules;
using yajusense.Utils;

namespace yajusense.UI;

public static class NotificationManager
{
    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }

    private const float DefaultDuration = 3f;
    private const float RectWidth = 400f;
    private const float RectHeight = 50f;
    private const float ProgressBarHeight = 2f;
    private const float Spacing = 10f;
    private const float PaddingX = 10f;
    private const float AnimationSpeed = 4f;
    private const int FontSize = 18;

    private static readonly List<Notification> Notifications = new();

    public static void ShowNotification(string message, NotificationType type = NotificationType.Info, float duration = -1)
    {
        Notifications.Insert(0, new Notification(message, type, duration > 0 ? duration : DefaultDuration));
    }

    public static void OnGUI()
    {
        Notifications.RemoveAll(n => n.IsScrollingOut && n.CurrentPosition.x >= Screen.width);

        foreach (Notification notification in Notifications)
        {
            if (notification.IsExpired && !notification.IsScrollingOut) notification.IsScrollingOut = true;

            float targetAlpha = notification.IsScrollingOut ? 0f : 1f;
            notification.CurrentAlpha = Mathf.Lerp(
                notification.CurrentAlpha,
                targetAlpha,
                AnimationSpeed * Time.deltaTime
            );
        }

        float screenRight = Screen.width;
        float screenBottom = Screen.height;

        for (var i = 0; i < Notifications.Count; i++)
        {
            Notification notification = Notifications[i];

            float targetX = notification.IsScrollingOut ? screenRight : screenRight - (RectWidth + Spacing);
            float targetY = screenBottom - (i + 1) * (RectHeight + Spacing);

            notification.CurrentPosition = Vector2.Lerp(
                notification.CurrentPosition,
                new Vector2(targetX, targetY),
                AnimationSpeed * Time.deltaTime
            );

            DrawNotification(notification, i);
        }
    }

    private static void DrawNotification(Notification notification, int displayIndex)
    {
        var rect = new Rect(notification.CurrentPosition, new Vector2(RectWidth, RectHeight));
        Drawer.DrawFilledRect(rect, new Color(0f, 0f, 0f, 0.3f * notification.CurrentAlpha));

        DrawProgressBar(rect, notification, displayIndex);
        DrawNotificationText(rect, notification);
    }

    private static void DrawProgressBar(Rect rect, Notification notification, int displayIndex)
    {
        float elapsedTime = Time.time - notification.StartTime;
        float progress = Mathf.Clamp01(elapsedTime / notification.Duration);
        float width = RectWidth * (1f - progress);

        Color progressBarColor = ColorUtils.GetRainbowColor(displayIndex * ModuleManager.ClientSettings.RainbowColorStep);
        progressBarColor.a = notification.CurrentAlpha;

        var progressBarRect = new Rect(
            rect.x,
            rect.yMax - ProgressBarHeight,
            width,
            ProgressBarHeight
        );

        Drawer.DrawFilledRect(progressBarRect, progressBarColor);
    }

    private static void DrawNotificationText(Rect rect, Notification notification)
    {
        Vector2 textSize = IMGUIUtils.CalcTextSize(notification.Message, FontSize);
        float textY = rect.center.y - textSize.y * 0.5f;

        Color textColor = notification.GetTextColor();
        textColor.a = notification.CurrentAlpha;

        Drawer.DrawText(
            notification.Message,
            new Vector2(rect.x + PaddingX, textY),
            textColor,
            FontSize
        );
    }

    private class Notification
    {
        public Notification(string message, NotificationType type, float duration)
        {
            Message = message;
            Type = type;
            CurrentPosition = new Vector2(Screen.width, Screen.height);
            CurrentAlpha = 0f;
            Duration = duration;
            StartTime = Time.time;
        }

        public string Message { get; }
        public NotificationType Type { get; }
        public Vector2 CurrentPosition { get; set; }
        public float CurrentAlpha { get; set; }
        public float Duration { get; }
        public float StartTime { get; }
        public bool IsScrollingOut { get; set; }

        public bool IsExpired => Time.time > StartTime + Duration;

        public Color GetTextColor()
        {
            return Type switch
            {
                NotificationType.Success => Color.green,
                NotificationType.Warning => Colors.Orange,
                NotificationType.Error => Color.red,
                _ => Color.white
            };
        }
    }
}