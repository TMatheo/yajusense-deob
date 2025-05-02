using ExitGames.Client.Photon;
using HarmonyLib;
using Il2CppSystem;
using yajusense.Modules;
using yajusense.Modules.Movement;

namespace yajusense.Patches;

[HarmonyPatch(typeof(LoadBalancingClient_Internal), nameof(LoadBalancingClient_Internal.Method_Public_Virtual_New_Boolean_Byte_Object_ObjectPublicObByObInByObObUnique_SendOptions_0))]
public static class OpRaiseEventPatch
{
	[HarmonyPrefix]
	public static bool Prefix(byte param_1, ref Object param_2, ObjectPublicObByObInByObObUnique param_3, SendOptions param_4)
	{
		if (param_1 == 12)
		{
			if (ModuleManager.GetModule<LagSwitch>().Enabled)
				return false;
		}

		return true;
	}
}