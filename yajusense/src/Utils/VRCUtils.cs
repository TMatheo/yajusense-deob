using VRC.Core;
using VRC.Localization;
using VRC.SDKBase;

namespace yajusense.Utils;

public static class VRCUtils
{
	public static LocalizableString CreateLocalizableString(string text)
	{
		return new LocalizableString(text, "", null, null, null);
	}

	public static int GetServerTimeMS()
	{
		return VRC.SDKBase.Networking.GetServerTimeInMilliseconds();
	}

	public static VRCPlayerApi GetLocalVRCPlayerApi()
	{
		if (VRC.SDKBase.Networking.LocalPlayer == null)
			return null;

		return VRC.SDKBase.Networking.LocalPlayer;
	}

	public static bool IsInWorld()
	{
		return GetLocalVRCPlayerApi() != null;
	}

	public static void ChangeAvatar(string avatarID)
	{
		var apiAvatar = new ApiAvatar
		{
			id = avatarID,
		};
		UsedToChangeAvatar.Method_Public_Static_Void_ApiAvatar_String_0(apiAvatar);
	}
}