using UnityEngine;

namespace yajusense.Networking;

public static class QuaternionSerializer
{
    private const float ConversionFactor = 0.00097751711f;

    public static byte[] Serialize(Quaternion quaternion)
    {
        var data = new byte[5];

        var x = (ushort)((quaternion.x + 1f) * 0.5f * 1023);
        var y = (ushort)((quaternion.y + 1f) * 0.5f * 1023);
        var z = (ushort)((quaternion.z + 1f) * 0.5f * 1023);
        var w = (ushort)((quaternion.w + 1f) * 0.5f * 1023);

        Pack10BitValues(data, 0, x, y, z, w);

        return data;
    }

    public static Quaternion Deserialize(byte[] data)
    {
        if (data == null || data.Length < 5) return Quaternion.identity;

        Unpack10BitValues(data, 0, out var x, out var y, out var z, out var w);

        var quaternion = new Quaternion
        {
            x = x * ConversionFactor * 2f - 1f,
            y = y * ConversionFactor * 2f - 1f,
            z = z * ConversionFactor * 2f - 1f,
            w = w * ConversionFactor * 2f - 1f
        };

        return quaternion;
    }

    private static void Pack10BitValues(byte[] buffer, int offset, ushort x, ushort y, ushort z, ushort w)
    {
        buffer[offset] = (byte)((x >> 2) & 0xFF);
        buffer[offset + 1] = (byte)(((x & 0x03) << 6) | ((y >> 4) & 0x3F));
        buffer[offset + 2] = (byte)(((y & 0x0F) << 4) | ((z >> 6) & 0x0F));
        buffer[offset + 3] = (byte)(((z & 0x3F) << 2) | ((w >> 8) & 0x03));
        buffer[offset + 4] = (byte)(w & 0xFF);
    }

    private static void Unpack10BitValues(byte[] buffer, int offset, out ushort x, out ushort y, out ushort z,
        out ushort w)
    {
        x = (ushort)((buffer[offset] << 2) | ((buffer[offset + 1] >> 6) & 0x03));
        y = (ushort)(((buffer[offset + 1] & 0x3F) << 4) | ((buffer[offset + 2] >> 4) & 0x0F));
        z = (ushort)(((buffer[offset + 2] & 0x0F) << 6) | ((buffer[offset + 3] >> 2) & 0x3F));
        w = (ushort)(((buffer[offset + 3] & 0x03) << 8) | buffer[offset + 4]);
    }
}