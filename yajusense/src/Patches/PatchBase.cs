using System;
using System.Reflection;
using HarmonyLib;

namespace yajusense.Patches;

public abstract class PatchBase
{
	private bool _isApplied;
	private HarmonyMethod _postfix;
	private HarmonyMethod _prefix;
	private HarmonyMethod _transpiler;

	protected PatchBase()
	{
		Initialize();
	}

	private MethodBase OriginalMethod { get; set; }
	private string PatchId => GetType().Name;

	protected abstract void Initialize();

	protected void ConfigurePatch(MethodBase original, string prefixName = null, string postfixName = null, string transpilerName = null)
	{
		HarmonyMethod prefix = prefixName != null ? CreatePatch(prefixName) : null;
		HarmonyMethod postfix = postfixName != null ? CreatePatch(postfixName) : null;
		HarmonyMethod transpiler = transpilerName != null ? CreatePatch(transpilerName) : null;

		ConfigurePatch(original, prefix, postfix, transpiler);
	}

	private void ConfigurePatch(MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
	{
		OriginalMethod = original;
		_prefix = prefix;
		_postfix = postfix;
		_transpiler = transpiler;
	}

	protected static void ApplyPatch<T>() where T : PatchBase, new()
	{
		var patch = new T();
		patch.Apply();
	}

	private HarmonyMethod CreatePatch(string methodName)
	{
		return new HarmonyMethod(GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance));
	}

	private void Apply()
	{
		if (_isApplied)
		{
			Plugin.Log.LogWarning($"Patch {PatchId} is already applied");
			return;
		}

		if (OriginalMethod == null)
		{
			Plugin.Log.LogError($"Failed to apply patch {PatchId}: Original method not found");
			return;
		}

		try
		{
			HarmonyPatcher.ApplyPatch(PatchId, OriginalMethod, _prefix, _postfix, _transpiler);
			_isApplied = true;
			Plugin.Log.LogInfo($"Successfully applied patch: {PatchId}");
		}
		catch (Exception ex)
		{
			Plugin.Log.LogError($"Failed to apply patch {PatchId}: {ex}");
		}
	}

	public void Remove()
	{
		if (!_isApplied)
			return;

		try
		{
			HarmonyPatcher.RemovePatch(PatchId);
			_isApplied = false;
			Plugin.Log.LogInfo($"Successfully removed patch: {PatchId}");
		}
		catch (Exception ex)
		{
			Plugin.Log.LogError($"Failed to remove patch {PatchId}: {ex}");
		}
	}
}