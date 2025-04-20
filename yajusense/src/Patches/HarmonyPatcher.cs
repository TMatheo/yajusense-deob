using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using yajusense.Core;

namespace yajusense.Patches;

public static class HarmonyPatcher
{
    private static readonly Dictionary<string, PatchInfo> Patches = new();
    private static Harmony _harmony;

    public static void Initialize()
    {
        _harmony = new Harmony("yajusense");
        YjPlugin.Log.LogInfo("HarmonyPatcher initialized");
    }

    public static void ApplyPatch(string patchId, MethodBase original, HarmonyMethod prefix = null,
        HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
    {
        if (Patches.ContainsKey(patchId))
        {
            YjPlugin.Log.LogWarning($"Patch {patchId} is already applied");
            return;
        }

        try
        {
            _harmony.Patch(original, prefix, postfix, transpiler);
            Patches.Add(patchId, new PatchInfo(original, prefix, postfix, transpiler));
            YjPlugin.Log.LogInfo($"Applied patch: {patchId}");
        }
        catch (Exception ex)
        {
            YjPlugin.Log.LogError($"Failed to apply patch {patchId}: {ex}");
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