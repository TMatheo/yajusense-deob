using System.Reflection;
using ExitGames.Client.Photon;
using HarmonyLib;
using Il2CppSystem;
using yajusense.Utils;

namespace yajusense.Patches;

public class OpRaiseEvent : PatchBase
{
	public static bool ShouldSendE12 { get; set; } = true;
	public static byte[] LastE12Data { get; private set; }

	protected override void Initialize()
	{
		MethodInfo originalMethod = AccessTools.Method(typeof(LoadBalancingClient_Internal), nameof(LoadBalancingClient_Internal.Method_Public_Virtual_New_Boolean_Byte_Object_ObjectPublicObByObInByObObUnique_SendOptions_0));

		ConfigurePatch(originalMethod, nameof(Prefix));
	}

	public static void ApplyPatch()
	{
		ApplyPatch<OpRaiseEvent>();
	}

	public static bool Prefix(byte param_1, ref Object param_2, ObjectPublicObByObInByObObUnique param_3, SendOptions param_4)
	{
		if (param_1 == 12)
		{
			byte[] data = Il2CppSerializationUtils.FromIL2CPPToManaged<byte[]>(param_2);
			LastE12Data = data;

			if (!ShouldSendE12)
				return false;
		}

		return true;
	}
}