using System.Collections.Generic;
using UnityEngine;
using Yjsnpi.Utilities;

namespace Yjsnpi.UI;

public static class NotificationManager
{
    private class Notification
    {
        public string Message { get; }
        public string FormattedMessage { get; }
        public float Duration { get; }
        public float StartTime { get; }
        public NotificationType Type { get; }
        public bool IsExpired => Time.time > StartTime + Duration;

        public Notification(string message, float duration, NotificationType type)
        {
            Message = message;
            Duration = duration;
            Type = type;
            StartTime = Time.time;
            FormattedMessage = FormatMessage(message, type);
        }

        private string FormatMessage(string message, NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => message.Color(RichTextUtility.Colors.Green),
                NotificationType.Warning => message.Color(RichTextUtility.Colors.Orange),
                NotificationType.Error => message.Color(RichTextUtility.Colors.Red),
                _ => message.Color(RichTextUtility.Colors.White)
            };
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
    private const float NotificationSpacing = 5f;
    private const float MaxWidth = 300f;
    private const float Margin = 10f;
    
    public static void AddNotification(string message, NotificationType type = NotificationType.Info, float duration = DefaultDuration)
    {
        Notifications.Add(new Notification(message, duration, type));
    }
    
    public static void OnGUI()
    {
        Notifications.RemoveAll(n => n.IsExpired);
        
        float startX = Screen.width - MaxWidth - Margin;
        float currentY = Screen.height - Margin;
        
        foreach (var notification in Notifications)
        {
            DrawNotification(notification, startX, ref currentY);
        }
    }

    private static void DrawNotification(Notification notification, float x, ref float y)
    {
        var content = new GUIContent(notification.FormattedMessage);
        var style = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(10, 10, 5, 5),
            wordWrap = true
        };
        
        var size = style.CalcSize(content);
        size.x = Mathf.Min(size.x, MaxWidth);
        size.y = style.CalcHeight(content, size.x);
        
        y -= size.y;
        var rect = new Rect(x, y, size.x, size.y);
        
        GUI.Box(rect, content, style);
        
        y -= NotificationSpacing;
    }
}