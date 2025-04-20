using System;
using UnityEngine;

namespace yajusense.Networking;

using System;
using UnityEngine;

public class QuaternionSerializer
{
    // Serializes a quaternion into a byte array
    // Each component is normalized to 0-1023 range and stored in 10 bits
    public static byte[] Serialize(Quaternion quaternion)
    {
        // Create a byte array to store the serialized quaternion
        // We need 5 bytes to store 4 components (10 bits each = 40 bits total)
        byte[] data = new byte[5];
        
        // Position tracker in bits
        int bitPosition = 0;
        
        // Process each component (x, y, z, w)
        WriteComponentToBits(data, NormalizeComponent(quaternion.x), ref bitPosition);
        WriteComponentToBits(data, NormalizeComponent(quaternion.y), ref bitPosition);
        WriteComponentToBits(data, NormalizeComponent(quaternion.z), ref bitPosition);
        WriteComponentToBits(data, NormalizeComponent(quaternion.w), ref bitPosition);
        
        return data;
    }
    
    // Deserializes a byte array back to a quaternion
    public static Quaternion Deserialize(byte[] data)
    {
        if (data == null || data.Length < 5)
            throw new ArgumentException("Invalid data array for deserialization");
            
        int bitPosition = 0;
        
        // Read each component (10 bits each)
        int xBits = ReadBitsFromArray(data, ref bitPosition, 10);
        int yBits = ReadBitsFromArray(data, ref bitPosition, 10);
        int zBits = ReadBitsFromArray(data, ref bitPosition, 10);
        int wBits = ReadBitsFromArray(data, ref bitPosition, 10);
        
        // Convert from 10-bit representation back to -1.0 to 1.0 range
        float x = DenormalizeComponent(xBits);
        float y = DenormalizeComponent(yBits);
        float z = DenormalizeComponent(zBits);
        float w = DenormalizeComponent(wBits);
        
        return new Quaternion(x, y, z, w);
    }
    
    // Converts a quaternion component from -1.0 to 1.0 range to a 10-bit integer (0-1023)
    private static int NormalizeComponent(float component)
    {
        // Same calculation as in the original code:
        // (component + 1.0) * 0.5 * 1023.0
        float normalized = (component + 1.0f) * 0.5f * 1023.0f;
        
        // Cap the value to the range 0-1023 (10 bits)
        return Mathf.Clamp((int)normalized, 0, 1023);
    }
    
    // Converts a 10-bit integer (0-1023) back to a quaternion component in -1.0 to 1.0 range
    private static float DenormalizeComponent(int bits)
    {
        // Reverse the normalization:
        // bits / 1023.0 * 2.0 - 1.0
        return (bits / 1023.0f) * 2.0f - 1.0f;
    }
    
    // Writes a 10-bit value to the byte array at the specified bit position
    private static void WriteComponentToBits(byte[] array, int value, ref int bitPosition)
    {
        // Ensure the value fits within 10 bits
        value &= 0x3FF;
        
        // Calculate which bytes this value will touch
        int startByteIndex = bitPosition / 8;
        int endByteIndex = (bitPosition + 9) / 8;
        int startBitOffset = bitPosition % 8;
        
        // Write the bits to the appropriate bytes
        for (int i = 0; i < 10; i++)
        {
            int byteIndex = (bitPosition + i) / 8;
            int bitOffset = (bitPosition + i) % 8;
            
            // If this bit in the value is set
            if ((value & (1 << (9 - i))) != 0)
            {
                // Set the corresponding bit in the array
                array[byteIndex] |= (byte)(1 << bitOffset);
            }
        }
        
        // Update the bit position
        bitPosition += 10;
    }
    
    // Reads a specified number of bits from the byte array at the specified bit position
    private static int ReadBitsFromArray(byte[] array, ref int bitPosition, int bitCount)
    {
        int result = 0;
        
        for (int i = 0; i < bitCount; i++)
        {
            int byteIndex = (bitPosition + i) / 8;
            int bitOffset = (bitPosition + i) % 8;
            
            // If the bit is set in the array
            if ((array[byteIndex] & (1 << bitOffset)) != 0)
            {
                // Set the corresponding bit in the result
                result |= (1 << (bitCount - 1 - i));
            }
        }
        
        // Update the bit position
        bitPosition += bitCount;
        return result;
    }
}