using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using yajusense.Core;
using yajusense.Modules;
using yajusense.Modules.Visual;
using yajusense.Patches;

namespace yajusense.Utils;

public static class CursorUnlocker
{
    private const string PatchIdCursorLock = "CursorLockStatePatch";
    private const string PatchIdCursorVisible = "CursorVisiblePatch";
    private static bool _currentlySettingCursor;
    private static CursorLockMode _lastLockMode = CursorLockMode.None;
    private static bool _lastVisibleState = true;
    private static Coroutine _unlockCoroutine;
    private static readonly WaitForEndOfFrame WaitForEndOfFrame = new();

    public static void Init()
    {
        _lastLockMode = Cursor.lockState;
        _lastVisibleState = Cursor.visible;

        ApplyPatches();
        UpdateCursorControl();

        if (_unlockCoroutine != null)
            CoroutineRunner.StopManagedCoroutine(_unlockCoroutine);

        _unlockCoroutine = CoroutineRunner.StartManagedCoroutine(UnlockCoroutine());
    }

    private static void ApplyPatches()
    {
        MethodInfo lockStateSetter = AccessTools.PropertySetter(typeof(Cursor), nameof(Cursor.lockState));
        if (lockStateSetter != null)
            HarmonyPatcher.ApplyPatch(PatchIdCursorLock, lockStateSetter, new HarmonyMethod(typeof(CursorUnlocker), nameof(Prefix_set_lockState)));
        else
            Plugin.Log.LogError("Failed to find Cursor.lockState setter for patching.");


        MethodInfo visibleSetter = AccessTools.PropertySetter(typeof(Cursor), nameof(Cursor.visible));
        if (visibleSetter != null)
            HarmonyPatcher.ApplyPatch(PatchIdCursorVisible, visibleSetter, new HarmonyMethod(typeof(CursorUnlocker), nameof(Prefix_set_visible)));
        else
            Plugin.Log.LogError("Failed to find Cursor.visible setter for patching.");
    }

    private static bool IsAnyUIShowing()
    {
        var menuModule = ModuleManager.GetModule<Menu>();
        var udonInspectorModule = ModuleManager.GetModule<UdonInspector>();

        bool menuEnabled = menuModule?.Enabled ?? false;
        bool udonInspectorEnabled = udonInspectorModule?.Enabled ?? false;

        return menuEnabled || udonInspectorEnabled;
    }

    private static IEnumerator UnlockCoroutine()
    {
        while (true)
        {
            yield return WaitForEndOfFrame;
            try
            {
                bool shouldBeUnlocked = IsAnyUIShowing();
                if (shouldBeUnlocked)
                {
                    if (Cursor.lockState != CursorLockMode.None || !Cursor.visible)
                        UpdateCursorControl();
                }
                else
                {
                    if (Cursor.lockState != _lastLockMode || Cursor.visible != _lastVisibleState)
                        UpdateCursorControl();
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Error in CursorUnlocker Coroutine: {ex}");
            }
        }
    }

    public static void UpdateCursorControl()
    {
        if (_currentlySettingCursor)
            return;

        try
        {
            _currentlySettingCursor = true;
            bool shouldUnlockNow = IsAnyUIShowing();

            CursorLockMode targetLockMode = shouldUnlockNow ? CursorLockMode.None : _lastLockMode;
            bool targetVisibleState = shouldUnlockNow || _lastVisibleState;

            if (Cursor.lockState != targetLockMode)
                Cursor.lockState = targetLockMode;

            if (Cursor.visible != targetVisibleState)
                Cursor.visible = targetVisibleState;
        }
        finally
        {
            _currentlySettingCursor = false;
        }
    }

    public static bool Prefix_set_lockState(ref CursorLockMode value)
    {
        if (_currentlySettingCursor)
            return true;

        if (!IsAnyUIShowing())
            _lastLockMode = value;

        if (IsAnyUIShowing())
            value = CursorLockMode.None;

        return true;
    }

    public static bool Prefix_set_visible(ref bool value)
    {
        if (_currentlySettingCursor)
            return true;

        if (!IsAnyUIShowing())
            _lastVisibleState = value;

        if (IsAnyUIShowing())
            value = true;

        return true;
    }
}