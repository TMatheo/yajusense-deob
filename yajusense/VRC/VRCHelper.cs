using UnityEngine;
using VRC.Core;
using VRC.Localization;
using VRC.SDKBase;

namespace yajusense.VRC;

public static class VRCHelper
{
	public static LocalizableString CreateLocalizableString(string text)
	{
		return new LocalizableString(text, "", null, null, null);
	}

	public static int GetServerTimeMS()
	{
		return global::VRC.SDKBase.Networking.GetServerTimeInMilliseconds();
	}

	public static VRCPlayerApi GetLocalVRCPlayerApi()
	{
		if (global::VRC.SDKBase.Networking.LocalPlayer == null)
			return null;

		return global::VRC.SDKBase.Networking.LocalPlayer;
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

	public static TrustRank GetTrustRank(APIUser user)
	{
		if (user.hasModerationPowers)
			return TrustRank.Moderator;

		if (user.hasNegativeTrustLevel)
			return TrustRank.Nuisance;

		if (user.hasVeteranTrustLevel)
			return TrustRank.TrustedUSer;

		if (user.hasTrustedTrustLevel)
			return TrustRank.KnownUser;

		if (user.hasKnownTrustLevel)
			return TrustRank.User;

		if (user.hasBasicTrustLevel)
			return TrustRank.NewUser;

		return TrustRank.Visitor;
	}


	public static Color GetUserColor(APIUser user)
	{
		if (APIUser.IsFriendsWith(user.id))
			return VRCPlayer_Internal.field_Internal_Static_Color_1;

		if (user.IsSelf)
			return VRCPlayer_Internal.field_Internal_Static_Color_0;

		switch (GetTrustRank(user))
		{
			case TrustRank.TrustedUSer:
				return VRCPlayer_Internal.field_Internal_Static_Color_6;

			case TrustRank.KnownUser:
				return VRCPlayer_Internal.field_Internal_Static_Color_5;

			case TrustRank.User:
				return VRCPlayer_Internal.field_Internal_Static_Color_4;

			case TrustRank.NewUser:
				return VRCPlayer_Internal.field_Internal_Static_Color_3;
		}

		return VRCPlayer_Internal.field_Internal_Static_Color_2;
	}
}