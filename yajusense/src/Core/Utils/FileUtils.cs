using System.IO;

namespace yajusense.Core.Utils;

public static class FileUtils
{
	public static void EnsureDirectoryExists(string dir)
	{
		if (!Directory.Exists(dir))
			Directory.CreateDirectory(dir);
	}
}