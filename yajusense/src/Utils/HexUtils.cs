using System;
using System.Text;
using UnityEngine;

namespace yajusense.Utils;

public static class HexUtils
{
    public static string ToHexString(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            return string.Empty;

        var hex = new StringBuilder(bytes.Length * 3);

        for (var i = 0; i < bytes.Length; i++)
        {
            hex.Append(bytes[i].ToString("X2"));

            if (i < bytes.Length - 1) hex.Append(' ');
        }

        return hex.ToString();
    }

    public static string ToHexString(Vector3 vector)
    {
        var xBytes = BitConverter.GetBytes(vector.x);
        var yBytes = BitConverter.GetBytes(vector.y);
        var zBytes = BitConverter.GetBytes(vector.z);

        var xHex = ToHexString(xBytes);
        var yHex = ToHexString(yBytes);
        var zHex = ToHexString(zBytes);

        return $"{xHex} {yHex} {zHex}";
    }
}