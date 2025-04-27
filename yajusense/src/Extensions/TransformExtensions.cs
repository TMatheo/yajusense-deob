using System.Text;
using UnityEngine;

namespace yajusense.Extensions;

public static class TransformExtensions
{
    public static string GetFullPath(this Transform transform)
    {
        StringBuilder path = new StringBuilder(transform.name);
        Transform current = transform.parent;

        while (current != null)
        {
            path.Insert(0, current.name + "/");
            current = current.parent;
        }

        return path.ToString();
    }
}