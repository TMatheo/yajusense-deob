using System;
using ExitGames.Client.Photon;
using yajusense.Utils;
using yajusense.Utils.VRC;

namespace yajusense.Networking;

public static class EventSender
{
	public static void SendVoice(byte[] voiceData)
	{
		var data = new byte[4 + voiceData.Length];

		byte[] header = BitConverter.GetBytes(NetworkingUtils.GetServerTimeMS());
		Buffer.BlockCopy(header, 0, data, 0, 4);

		Buffer.BlockCopy(voiceData, 0, data, 4, voiceData.Length);

		RaiseEvent((byte)PhotonEventType.Voice, data, false);
	}

	private static void RaiseEvent(byte code, object content, bool isReliable)
	{
		PhotonNetwork_Internal.Method_Public_Static_Boolean_Byte_Object_ObjectPublicObByObInByObObUnique_SendOptions_0(code, Il2CppSerializationUtils.FromManagedToIL2CPP<Il2CppSystem.Object>(content), null, isReliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
	}
}