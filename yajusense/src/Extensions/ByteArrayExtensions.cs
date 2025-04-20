using System.Text;

namespace yajusense.Extensions;

public static class ByteArrayExtensions
{
    public static string ToHexString(this byte[] bytes, bool upperCase = true)
    {
        if (bytes == null || bytes.Length == 0)
            return string.Empty;

        var hexFormat = upperCase ? "X2" : "x2";
        var hex = new StringBuilder(bytes.Length * 3);

        for (int i = 0; i < bytes.Length; i++)
        {
            hex.Append(bytes[i].ToString(hexFormat));
            
            if (i < bytes.Length - 1)
            {
                hex.Append(' ');
            }
        }

        return hex.ToString();
    }
}