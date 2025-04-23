using ExitGames.Client.Photon;
using HarmonyLib;
using Il2CppSystem;
using yajusense.Utils;

namespace yajusense.Patches;

public class OpRaiseEventPatch : BasePatch
{
    public static bool ShouldSendE12 { get; set; } = true;
    public static byte[] LastData { get; private set; }

    protected override void Initialize()
    {
        var originalMethod = AccessTools.Method(typeof(LoadBalancingClient_Internal),
            nameof(LoadBalancingClient_Internal
                .Method_Public_Virtual_New_Boolean_Byte_Object_ObjectPublicObByObInByObObUnique_SendOptions_0));

        ConfigurePatch(originalMethod, nameof(Prefix));
    }

    public static void ApplyPatch()
    {
        ApplyPatch<OpRaiseEventPatch>();
    }

    public static bool Prefix(byte param_1, Object param_2, ObjectPublicObByObInByObObUnique param_3,
        SendOptions param_4)
    {
        if (param_1 == 12)
        {
            var data = Il2CppSerializationUtils.FromIL2CPPToManaged<byte[]>(param_2);
            LastData = data;

            if (!ShouldSendE12)
                return false;
        }

        return true;
    }
}