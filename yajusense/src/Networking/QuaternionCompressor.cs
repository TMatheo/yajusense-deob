using System;
using UnityEngine;
using yajusense.Core;

namespace yajusense.Networking;

public static class QuaternionCompressor
{
    public static byte[] CompressQuaternion(Quaternion quat)
    {
        byte[] buffer = new byte[5];
        int offset = 0;
        
        EncodeComponent(buffer, ref offset, (quat.x + 1f) * 0.5f * 1023f);
        EncodeComponent(buffer, ref offset, (quat.y + 1f) * 0.5f * 1023f);
        EncodeComponent(buffer, ref offset, (quat.z + 1f) * 0.5f * 1023f);
        EncodeComponent(buffer, ref offset, (quat.w + 1f) * 0.5f * 1023f);

        return buffer;
    }
    
    private static void EncodeComponent(byte[] buffer, ref int offset, float value)
    {
        int intValue = Mathf.RoundToInt(value) & 0x3FF;
        
        switch (offset % 4)
        {
            case 0:
                buffer[offset / 4] = (byte)(intValue >> 2);
                buffer[offset / 4 + 1] |= (byte)((intValue & 0x03) << 6);
                break;
            case 1:
                buffer[offset / 4] |= (byte)(intValue << 0);
                buffer[offset / 4 + 1] |= (byte)((intValue & 0x3F) << 2);
                break;
            case 2:
                buffer[offset / 4] |= (byte)(intValue >> 6);
                buffer[offset / 4 + 1] |= (byte)((intValue & 0x3F) << 2);
                break;
            case 3:
                buffer[offset / 4] |= (byte)(intValue >> 4);
                buffer[offset / 4 + 1] |= (byte)((intValue & 0x0F) << 4);
                break;
        }

        offset++;
    }
    
    public static Quaternion DecompressQuaternion(byte[] data)
    {
        if (data.Length != 5)
            YjPlugin.Log.LogError("Input data must be 5 bytes long");

        int offset = 0;
        return new Quaternion(
            DecodeComponent(data, ref offset),
            DecodeComponent(data, ref offset),
            DecodeComponent(data, ref offset),
            DecodeComponent(data, ref offset)
        );
    }
    
    private static float DecodeComponent(byte[] data, ref int offset)
    {
        int value = 0;

        switch (offset % 4)
        {
            case 0:
                value = (data[offset / 4] << 2) | (data[offset / 4 + 1] >> 6);
                break;
            case 1:
                value = (data[offset / 4] >> 0) | ((data[offset / 4 + 1] & 0x3F) << 2);
                break;
            case 2:
                value = (data[offset / 4] << 6) | ((data[offset / 4 + 1] & 0x3F) << 2);
                break;
            case 3:
                value = (data[offset / 4] << 4) | ((data[offset / 4 + 1] & 0x0F) << 4);
                break;
        }

        offset++;
        return value / 1023f * 2f - 1f;
    }
}