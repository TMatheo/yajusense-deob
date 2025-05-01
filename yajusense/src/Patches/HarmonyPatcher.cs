using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace yajusense.Patches;

public static class HarmonyPatcher
{
	private static readonly Dictionary<string, PatchInfo> Patches = new();
	private static Harmony _harmony;

	public static void Initialize()
	{
		_harmony = new Harmony("yajusense");
		Plugin.Log.LogInfo("HarmonyPatcher initialized");
	}

	public static void ApplyPatch(string patchId, MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
	{
		if (Patches.ContainsKey(patchId))
		{
			Plugin.Log.LogWarning($"Patch {patchId} is already applied");
			return;
		}

		try
		{
			_harmony.Patch(original, prefix, postfix, transpiler);
			Patches.Add(patchId, new PatchInfo(original, prefix, postfix, transpiler));
			Plugin.Log.LogInfo($"Applied patch: {patchId}");
		}
		catch (Exception ex)
		{
			Plugin.Log.LogError($"Failed to apply patch {patchId}: {ex}");
		}
	}

	public static void RemovePatch(string patchId)
	{
		if (!Patches.TryGetValue(patchId, out PatchInfo patchInfo))
		{
			Plugin.Log.LogWarning($"Patch {patchId} not found");
			return;
		}

		try
		{
			_harmony.Unpatch(patchInfo.Original, patchInfo.Prefix?.method);
			_harmony.Unpatch(patchInfo.Original, patchInfo.Postfix?.method);
			_harmony.Unpatch(patchInfo.Original, patchInfo.Transpiler?.method);

			Patches.Remove(patchId);
			Plugin.Log.LogInfo($"Removed patch: {patchId}");
		}
		catch (Exception ex)
		{
			Plugin.Log.LogError($"Failed to remove patch {patchId}: {ex}");
		}
	}

	private class PatchInfo
	{
		public PatchInfo(MethodBase original, HarmonyMethod prefix, HarmonyMethod postfix, HarmonyMethod transpiler)
		{
			Original = original;
			Prefix = prefix;
			Postfix = postfix;
			Transpiler = transpiler;
		}

		public MethodBase Original { get; }
		public HarmonyMethod Prefix { get; }
		public HarmonyMethod Postfix { get; }
		public HarmonyMethod Transpiler { get; }
	}
}