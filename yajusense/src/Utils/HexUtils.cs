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

        for (int i = 0; i < bytes.Length; i++)
        {
            hex.Append(bytes[i].ToString("X2"));
            
            if (i < bytes.Length - 1)
            {
                hex.Append(' ');
            }
        }

        return hex.ToString();
    }
    
    public static string ToHexString(Vector3 vector)
    {
        byte[] xBytes = BitConverter.GetBytes(vector.x);
        byte[] yBytes = BitConverter.GetBytes(vector.y);
        byte[] zBytes = BitConverter.GetBytes(vector.z);
        
        string xHex = ToHexString(xBytes);
        string yHex = ToHexString(yBytes);
        string zHex = ToHexString(zBytes);
        
        return $"{xHex} {yHex} {zHex}";
    }
}