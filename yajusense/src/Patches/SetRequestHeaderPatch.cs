using HarmonyLib;
using UnityEngine.Networking;

 namespace yajusense.Patches;

 [HarmonyPatch(typeof(UnityWebRequest), nameof(UnityWebRequest.SetRequestHeader))]
 public static class SetRequestHeaderPatch
 {
 	[HarmonyPostfix]
 	public static void Postfix(string name, string value)
 	{
 		Plugin.Log.LogInfo($"SetRequestHeader: {name} {value}");
 	}
}

