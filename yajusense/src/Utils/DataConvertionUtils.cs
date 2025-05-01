using System;
using System.Text;
using UnityEngine;

namespace yajusense.Utils;

public static class DataConvertionUtils
{
	public static string ToHexString(byte[] bytes)
	{
		if (bytes == null || bytes.Length == 0)
			return string.Empty;

		var hex = new StringBuilder(bytes.Length * 3);

		for (var i = 0; i < bytes.Length; i++)
		{
			hex.Append(bytes[i].ToString("X2"));

			if (i < bytes.Length - 1)
				hex.Append(' ');
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

	public static byte[] Vector3ToBytes(Vector3 vector)
	{
		var result = new byte[12];

		Buffer.BlockCopy(BitConverter.GetBytes(vector.x), 0, result, 0, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(vector.y), 0, result, 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(vector.z), 0, result, 8, 4);

		return result;
	}

	public static byte[] QuaternionToBytes(Quaternion quaternion)
	{
		var bytes = new byte[16];

		Buffer.BlockCopy(BitConverter.GetBytes(quaternion.x), 0, bytes, 0, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(quaternion.y), 0, bytes, 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(quaternion.z), 0, bytes, 8, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(quaternion.w), 0, bytes, 12, 4);

		return bytes;
	}
}