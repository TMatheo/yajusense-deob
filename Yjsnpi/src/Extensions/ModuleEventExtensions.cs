using System;
using Yjsnpi.Events;
using Yjsnpi.Modules;

namespace Yjsnpi.Extensions;

public static class ModuleEventExtensions
{
    public static void SubscribeEvent<T>(this BaseModule module, Action<T> handler)
    {
        EventManager.AddEventListener(handler);
    }
    
    public static void PublishEvent<T>(this BaseModule module, T eventData)
    {
        EventManager.Publish(eventData);
    }
}