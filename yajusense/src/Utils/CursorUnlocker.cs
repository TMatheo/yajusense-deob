using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;
using yajusense.Core;
using yajusense.Modules;
using yajusense.Modules.Visual;
using yajusense.Modules.Visual.HUD;

namespace yajusense.Utils
{
    public static class CursorUnlocker
    {
        public static bool ForceUnlockMouse { get; set; } = true;

        private static bool ShouldUnlock => ForceUnlockMouse && IsAnyUIShowing();

        private static bool _currentlySettingCursor;
        private static CursorLockMode _lastLockMode = CursorLockMode.None;
        private static bool _lastVisibleState = true;

        private static Coroutine _unlockCoroutine;
        private static readonly WaitForEndOfFrame WaitForEndOfFrame = new();
        
        private static Harmony _harmony;

        public static void Init(Harmony harmonyInstance)
        {
            _harmony = harmonyInstance ?? throw new ArgumentNullException(nameof(harmonyInstance));

            try
            {
                _lastLockMode = Cursor.lockState;
                _lastVisibleState = Cursor.visible;

                InitPatches();
                UpdateCursorControl();

                if (_unlockCoroutine != null)
                    CoroutineRunner.StopManagedCoroutine(_unlockCoroutine);
                _unlockCoroutine = CoroutineRunner.StartManagedCoroutine(UnlockCoroutine());

                YjPlugin.Log.LogInfo("CursorUnlocker initialized.");
            }
            catch (Exception ex)
            {
                YjPlugin.Log.LogWarning($"Exception initializing CursorUnlocker: {ex}");
            }
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

                bool shouldBeUnlocked = ShouldUnlock;
                if (shouldBeUnlocked)
                {
                    if (Cursor.lockState != CursorLockMode.None || !Cursor.visible)
                    {
                        UpdateCursorControl();
                    }
                }
                else
                {
                    if (Cursor.lockState != _lastLockMode || Cursor.visible != _lastVisibleState)
                    {
                        UpdateCursorControl();
                    }
                }
            }
        }
        
        public static void UpdateCursorControl()
        {
            if (_currentlySettingCursor) return;

            try
            {
                _currentlySettingCursor = true;
                bool shouldUnlockNow = ShouldUnlock;

                if (shouldUnlockNow)
                {
                    if (Cursor.lockState != CursorLockMode.None)
                    {
                        Cursor.lockState = CursorLockMode.None;
                    }
                    if (!Cursor.visible)
                    {
                        Cursor.visible = true;
                    }
                }
                else
                {
                    if (Cursor.lockState != _lastLockMode)
                    {
                        Cursor.lockState = _lastLockMode;
                    }
                    if (Cursor.visible != _lastVisibleState)
                    {
                        Cursor.visible = _lastVisibleState;
                    }
                }
            }
            finally
            {
                _currentlySettingCursor = false;
            }
        }

        private static void InitPatches()
        {
            if (_harmony == null)
            {
                YjPlugin.Log.LogError("Harmony instance is null in CursorUnlocker. Cannot apply patches.");
                return;
            }

            try
            {
                YjPlugin.Log.LogInfo("Applying Cursor patches...");
                
                var lockStateSetter = AccessTools.PropertySetter(typeof(Cursor), nameof(Cursor.lockState));
                if (lockStateSetter != null)
                {
                    _harmony.Patch(lockStateSetter,
                        prefix: new HarmonyMethod(typeof(CursorUnlocker), nameof(Prefix_set_lockState)));
                    YjPlugin.Log.LogDebug("Patched Cursor.lockState setter.");
                }
                else
                {
                    YjPlugin.Log.LogWarning("Could not find Cursor.lockState setter.");
                }
                
                var visibleSetter = AccessTools.PropertySetter(typeof(Cursor), nameof(Cursor.visible));
                if (visibleSetter != null)
                {
                    _harmony.Patch(visibleSetter,
                        prefix: new HarmonyMethod(typeof(CursorUnlocker), nameof(Prefix_set_visible)));
                    YjPlugin.Log.LogDebug("Patched Cursor.visible setter.");
                }
                else
                {
                    YjPlugin.Log.LogWarning("Could not find Cursor.visible setter.");
                }
            }
            catch (Exception ex)
            {
                YjPlugin.Log.LogError($"Failed to apply Cursor patches: {ex}");
            }
        }
        
        public static bool Prefix_set_lockState(ref CursorLockMode value)
        {
            if (_currentlySettingCursor)
            {
                return true;
            }
            
            _lastLockMode = value;
            
            
            if (ShouldUnlock)
            {
                value = CursorLockMode.None;
            }
            return true;
        }
        
        public static bool Prefix_set_visible(ref bool value)
        {
            if (_currentlySettingCursor)
            {
                return true;
            }
            
            _lastVisibleState = value;
            
            if (ShouldUnlock)
            {
                if (!value)
                {
                    value = true;
                }
            }

            return true;
        }
    }
}