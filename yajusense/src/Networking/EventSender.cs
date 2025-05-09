using System;
using ExitGames.Client.Photon;
using yajusense.Utils;
using yajusense.VRC;

namespace yajusense.Networking;

public static class EventSender
{
	// you can generate a payload with photon_event1_generator.py
	public static void SendVoice(string payloadBase64)
	{
		byte[] payload = Convert.FromBase64String(payloadBase64);

		byte[] serverTimeBytes = BitConverter.GetBytes(VRCHelper.GetServerTimeMS());
		Buffer.BlockCopy(serverTimeBytes, 0, payload, 0, 4);

		RaiseEvent((byte)PhotonEventType.Voice, payload, false);
	}

	private static void RaiseEvent(byte code, object content, bool isReliable)
	{
		PhotonNetwork_Internal.Method_Public_Static_Boolean_Byte_Object_ObjectPublicObByObInByObObUnique_SendOptions_0(code, Il2CppSerializationUtils.FromManagedToIL2CPP<Il2CppSystem.Object>(content), null, isReliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
	}
}