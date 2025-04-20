// pasted

namespace yajusense.Utils;

public static class SerializationUtils
{
    public static T FromIL2CPPToManaged<T>(Il2CppSystem.Object obj)
    {
        return FromByteArray<T>(ToByteArray(obj));
    }

    public static T FromManagedToIL2CPP<T>(object obj)
    {
        return IL2CPPFromByteArray<T>(ToByteArray(obj));
    }
    
    private static byte[] ToByteArray(Il2CppSystem.Object obj)
    {
        if (obj == null) return null;
        var bf = new Il2CppSystem.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        var ms = new Il2CppSystem.IO.MemoryStream();
        bf.Serialize(ms, obj);
        return ms.ToArray();
    }

    private static byte[] ToByteArray(object obj)
    {
        if (obj == null) return null;
        var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        var ms = new System.IO.MemoryStream();
        bf.Serialize(ms, obj);
        return ms.ToArray();
    }

    private static T FromByteArray<T>(byte[] data)
    {
        if (data == null) return default(T);
        var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        var ms = new System.IO.MemoryStream(data);
        var obj = bf.Deserialize(ms);
        return (T)obj;
    }

    private static T IL2CPPFromByteArray<T>(byte[] data)
    {
        if (data == null) return default(T);
        var bf = new Il2CppSystem.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        var ms = new Il2CppSystem.IO.MemoryStream(data);
        object obj = bf.Deserialize(ms);
        return (T)obj;
    }
}