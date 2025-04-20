using System;
using System.Collections;
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

        var lockStateSetter = AccessTools.PropertySetter(typeof(Cursor), nameof(Cursor.lockState));
        if (lockStateSetter != null)
        {
            HarmonyPatcher.ApplyPatch(
                PatchIdCursorLock,
                lockStateSetter,
                new HarmonyMethod(typeof(CursorUnlocker).GetMethod(nameof(Prefix_set_lockState)))
            );
        }
            
        var visibleSetter = AccessTools.PropertySetter(typeof(Cursor), nameof(Cursor.visible));
        if (visibleSetter != null)
        {
            HarmonyPatcher.ApplyPatch(
                PatchIdCursorVisible,
                visibleSetter,
                new HarmonyMethod(typeof(CursorUnlocker).GetMethod(nameof(Prefix_set_visible)))
            );
        }
    }

    private static bool IsAnyUIShowing()
    {
        var menuModule = ModuleManager.GetModule<Menu>();
        var udonInspectorModule = ModuleManager.GetModule<UdonInspector>();

        var menuEnabled = menuModule?.Enabled ?? false;
        var udonInspectorEnabled = udonInspectorModule?.Enabled ?? false;

        return menuEnabled || udonInspectorEnabled;
    }

    private static IEnumerator UnlockCoroutine()
    {
        while (true)
        {
            yield return WaitForEndOfFrame;

            var shouldBeUnlocked = IsAnyUIShowing();
            if (shouldBeUnlocked)
            {
                if (Cursor.lockState != CursorLockMode.None || !Cursor.visible) UpdateCursorControl();
            }
            else
            {
                if (Cursor.lockState != _lastLockMode || Cursor.visible != _lastVisibleState) UpdateCursorControl();
            }
        }
    }

    public static void UpdateCursorControl()
    {
        if (_currentlySettingCursor) return;

        try
        {
            _currentlySettingCursor = true;
            var shouldUnlockNow = IsAnyUIShowing();

            if (shouldUnlockNow)
            {
                if (Cursor.lockState != CursorLockMode.None) Cursor.lockState = CursorLockMode.None;

                if (!Cursor.visible) Cursor.visible = true;
            }
            else
            {
                if (Cursor.lockState != _lastLockMode) Cursor.lockState = _lastLockMode;

                if (Cursor.visible != _lastVisibleState) Cursor.visible = _lastVisibleState;
            }
        }
        finally
        {
            _currentlySettingCursor = false;
        }
    }

    public static bool Prefix_set_lockState(ref CursorLockMode value)
    {
        if (_currentlySettingCursor) return true;

        _lastLockMode = value;

        if (IsAnyUIShowing()) value = CursorLockMode.None;

        return true;
    }

    public static bool Prefix_set_visible(ref bool value)
    {
        if (_currentlySettingCursor) return true;

        _lastVisibleState = value;

        if (IsAnyUIShowing())
            if (!value)
                value = true;

        return true;
    }
}