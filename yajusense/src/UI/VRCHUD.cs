using UnityEngine;
using VRC.UI.Client.HUD;
using yajusense.Platform.VRC;

namespace yajusense.UI;

public static class VRCHUD
{
	public static void ShowNotification(string message, Sprite icon = null, float duration = 3f)
	{
		HudController hudController = GetHudControllerInstance();
		if (hudController == null)
			return;

		hudController.Method_Public_Void_LocalizableString_Sprite_Single_0(VRCHelper.CreateLocalizableString(message), icon, duration);
	}

	private static HudController GetHudControllerInstance()
	{
		return HudController.field_Public_Static_HudController_0;
	}
}