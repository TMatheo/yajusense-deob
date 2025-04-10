using System;

namespace Yjsnpi.Core.Config;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ConfigAttribute : Attribute
{
    public string DisplayName { get; }
    public string Description { get; }
        
    public ConfigAttribute(string displayName, string description = "")
    {
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Description = description;
    }
}
