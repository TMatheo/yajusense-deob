using UnityEngine;

namespace yajusense.Utils.VRC;

public static class HUDUtils
{
	public static void ShowNotification(string message, Sprite icon = null, float duration = 3f)
	{
		HudController_Internal hudController = GetHudControllerInstance();
		if (hudController == null)
			return;

		hudController.Method_Public_Void_LocalizableString_Sprite_Single_0(LocalizableStringUtils.Create(message), icon, duration);
	}

	private static HudController_Internal GetHudControllerInstance()
	{
		return HudController_Internal.field_Public_Static_MonoBehaviourPublicObnoObmousCaObhuGa_gUnique_0;
	}
}