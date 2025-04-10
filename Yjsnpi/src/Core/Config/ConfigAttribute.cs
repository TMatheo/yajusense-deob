using System;

namespace Yjsnpi.Core.Config;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ConfigAttribute : Attribute
{
    public string DisplayName { get; }
    public string Description { get; }
    public float Min { get; }
    public float Max { get; }
        
    public ConfigAttribute(string displayName, string description = "", float min = float.MinValue, float max = float.MaxValue)
    {
        DisplayName = displayName;
        Description = description;
        Min = min;
        Max = max;
    }
}
