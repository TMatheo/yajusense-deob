using System.Collections.Generic;
using UnityEngine;
using Yjsnpi.Utils;

namespace Yjsnpi.UI;

public static class NotificationManager
{
    private class Notification
    {
        public string Message { get; }
        public NotificationType Type { get; }
        public Vector2 CurrentPos { get; set; }
        public float Duration { get; }
        public float StartTime { get; }
        public int OriginalIndex { get; }
        public bool IsScrollingOut { get; set; }
        
        public bool IsExpired => Time.time > StartTime + Duration;

        public Notification(string message, NotificationType type, float duration, int index)
        {
            Message = message;
            Type = type;
            CurrentPos = new(Screen.width, Screen.height);
            Duration = duration;
            StartTime = Time.time;
            OriginalIndex = index;
            IsScrollingOut = false;
        }

        public Color GetTextColor()
        {
            switch (Type)
            {
                case NotificationType.Success:
                    return Color.green;
                case NotificationType.Warning:
                    return Colors.Orange;
                case NotificationType.Error:
                    return Color.red;
                default:
                    return Color.white;
            }
        }
    }
    
    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }
    
    private static readonly List<Notification> Notifications = new();
    private const float DefaultDuration = 3f;
    private const float RectSizeX = 400f;
    private const float RectSizeY = 50f;
    private const float ProgressBarSizeY = 2f;
    private const float Spacing = 20f;
    private const float PaddingX = 10f;
    private const float AnimSpeed = 4f;
    private const int FontSize = 18;
    
    public static void ShowNotification(string message, NotificationType type = NotificationType.Info, float duration = DefaultDuration)
    {
        Notifications.Add(new Notification(message, type, duration, Notifications.Count));
    }
    
    public static void OnGUI()
    {
        Notifications.RemoveAll(n => n.IsScrollingOut && n.CurrentPos.x >= Screen.width - 50);
        
        foreach (var notification in Notifications.FindAll(n => n.IsExpired && !n.IsScrollingOut))
        {
            notification.IsScrollingOut = true;
        }

        int index = 0;
        foreach (var notification in Notifications)
        {
            float targetRectX, targetRectY;
            
            if (notification.IsScrollingOut)
            {
                targetRectX = Screen.width;
                targetRectY = Screen.height;
            }
            else
            {
                targetRectX = Screen.width - (RectSizeX + Spacing);
                targetRectY = Screen.height - (index + 1) * (RectSizeY + Spacing);
            }
            
            notification.CurrentPos = Vector2.Lerp(notification.CurrentPos, new(targetRectX, targetRectY), AnimSpeed * Time.deltaTime);

            Rect rect = new(notification.CurrentPos, new Vector2(RectSizeX, RectSizeY));
            Drawer.DrawFilledRect(rect, new(0f, 0f, 0f, 0.3f));
            
            Vector2 progressBarPos = new(rect.x, rect.yMax - ProgressBarSizeY);
            float timeElapsed = Time.time - notification.StartTime;
            float progress = Mathf.Clamp01(timeElapsed / notification.Duration);
            Vector2 progressBarSize = new(RectSizeX * (1f - progress), ProgressBarSizeY);
            Rect progressBarRect = new(progressBarPos, progressBarSize);
            Drawer.DrawFilledRect(progressBarRect, ColorUtils.GetRainbowColor(index * 0.05f));
            
            string text = notification.Message;
            Vector2 textSize = IMGUIUtils.CalcTextSize(text, FontSize);
            float textPosY = rect.center.y - textSize.y * 0.5f;
            Vector2 textPos = new(rect.x + PaddingX, textPosY);
            Drawer.DrawText(text, textPos, notification.GetTextColor(), FontSize);
            
            index++;
        }
    }
}