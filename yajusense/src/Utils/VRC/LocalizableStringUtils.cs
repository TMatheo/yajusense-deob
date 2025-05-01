using VRC.Localization;

namespace yajusense.Utils.VRC;

public static class LocalizableStringUtils
{
	public static LocalizableString Create(string text)
	{
		return new LocalizableString(text, "", null, null, null);
	}
}