using ExitGames.Client.Photon;
using yajusense.Utils;
using Object = Il2CppSystem.Object;

namespace yajusense.Networking;

public static class EventSender
{
    private static void RaiseEvent(byte code, object content, bool isReliable)
    {
        PhotonNetwork_Internal.Method_Public_Static_Boolean_Byte_Object_ObjectPublicObByObInByObObUnique_SendOptions_0(code, Il2CppSerializationUtils.FromManagedToIL2CPP<Object>(content), null, isReliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
    }
}