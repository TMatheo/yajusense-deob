using ExitGames.Client.Photon;
using HarmonyLib;
using Il2CppSystem;
using yajusense.Modules;
using yajusense.Modules.Movement;
using yajusense.Networking;

namespace yajusense.Patches;

[HarmonyPatch(typeof(LoadBalancingClient_Internal), nameof(LoadBalancingClient_Internal.Method_Public_Virtual_New_Boolean_Byte_Object_ObjectPublicObByObInByObObUnique_SendOptions_0))]
public static class OpRaiseEventPatch
{
	[HarmonyPrefix]
	public static bool Prefix(byte param_1, ref Object param_2, ObjectPublicObByObInByObObUnique param_3, SendOptions param_4)
	{
		ModuleManager.NotifyOpRaiseEvent(param_1, ref param_2);

		if (param_1 == (byte)PhotonEventType.PlayerData)
		{
			if (ModuleManager.GetModule<LagSwitch>().Enabled)
				return false;
		}

		// if (param_1 == 1)
		// {
		// 	byte[] data = Il2CppSerializationUtils.FromIL2CPPToManaged<byte[]>(param_2);
		// 	Plugin.Log.LogInfo($"E1: {DataConversionUtils.ToHexString(data)}");
		// 	
		// 	byte[] serverTimeBytes = BitConverter.GetBytes(NetworkingUtils.GetServerTimeMS());
		// 	Plugin.Log.LogInfo($"ServerTime: {DataConversionUtils.ToHexString(serverTimeBytes)}");
		// }

		return true;
	}
}