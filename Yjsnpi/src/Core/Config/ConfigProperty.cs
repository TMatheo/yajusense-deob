using System.Reflection;

namespace Yjsnpi.Core.Config;

public class ConfigProperty
{
    public PropertyInfo Property { get; }
    public ConfigAttribute Attribute { get; }

    public ConfigProperty(PropertyInfo property, ConfigAttribute attribute)
    {
        Property = property;
        Attribute = attribute;
    }
}