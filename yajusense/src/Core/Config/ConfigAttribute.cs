using System;

namespace yajusense.Core.Config;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ConfigAttribute : Attribute
{
	public ConfigAttribute(string displayName, string description = "", bool hidden = false, float min = float.MinValue, float max = float.MaxValue)
	{
		DisplayName = displayName;
		Description = description;
		Hidden = hidden;
		Min = min;
		Max = max;
	}

	public string DisplayName { get; }
	public string Description { get; }
	public bool Hidden { get; }
	public float Min { get; }
	public float Max { get; }
}