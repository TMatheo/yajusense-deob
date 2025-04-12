using System.IO;

namespace Yjsnpi.Utils;

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