using System;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using yajusense.Core;

namespace yajusense.Patches;

public abstract class BasePatch
{
    protected static ManualLogSource Log => YjPlugin.Log;
    
    private bool _isApplied;
    private HarmonyMethod _prefix;
    private HarmonyMethod _postfix;
    private HarmonyMethod _transpiler;
    
    protected BasePatch()
    {
        Initialize();
    }
    
    protected abstract void Initialize();
    
    protected void ConfigurePatch(MethodBase original, HarmonyMethod prefix = null, 
        HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
    {
        OriginalMethod = original;
        _prefix = prefix;
        _postfix = postfix;
        _transpiler = transpiler;
    }
    
    public MethodBase OriginalMethod { get; private set; }
    public string PatchId => GetType().Name;
    
    public void Apply()
    {
        if (_isApplied)
        {
            Log.LogWarning($"Patch {PatchId} is already applied");
            return;
        }

        if (OriginalMethod == null)
        {
            Log.LogError($"Failed to apply patch {PatchId}: Original method not found");
            return;
        }

        try
        {
            HarmonyPatcher.ApplyPatch(PatchId, OriginalMethod, _prefix, _postfix, _transpiler);
            _isApplied = true;
        }
        catch (Exception ex)
        {
            Log.LogError($"Failed to apply patch {PatchId}: {ex}");
        }
    }
    
    public void Remove()
    {
        if (!_isApplied) return;
        
        try
        {
            HarmonyPatcher.RemovePatch(PatchId);
            _isApplied = false;
        }
        catch (Exception ex)
        {
            Log.LogError($"Failed to remove patch {PatchId}: {ex}");
        }
    }
}