using System.IO;

namespace yajusense.Utils;

public static class FileUtils
{
    public static void EnsureDirectoryExists(string dir)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }
}