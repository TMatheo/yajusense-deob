using System;
using Yjsnpi.Events;
using Yjsnpi.Modules;

namespace Yjsnpi.Extensions;

public static class ModuleEventExtensions
{
    public static void Subscribe<T>(this BaseModule module, Action<T> handler) where T : struct
    {
        EventManager.Subscribe(handler);
    }

    public static void SendEvent<T>(this BaseModule module, in T eventData) where T : struct
    {
        EventManager.Send(in eventData);
    }
}