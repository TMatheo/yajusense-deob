using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExitGames.Client.Photon;
using HarmonyLib;
using UnityEngine;
using VRC.SDKBase;
using yajusense.Core.Services;
using yajusense.Modules;
using yajusense.Modules.Visual;
using yajusense.UI;
using yajusense.Utils;

namespace yajusense.Patches;

public class NetworkManagerOnEvent : PatchBase
{
    private const float CrashThreshold = 4f;
    private static readonly Dictionary<VRCPlayerApi, float> LastEvent12Time = new();

    public static bool IsPlayerCrashed(VRCPlayerApi player)
    {
        if (!LastEvent12Time.TryGetValue(player, out float lastEventTime))
            return false;

        return Time.time - lastEventTime >= CrashThreshold;
    }

    protected override void Initialize()
    {
        MethodInfo originalMethod = AccessTools.Method(
            typeof(NetworkManager_Internal),
            nameof(NetworkManager_Internal.Method_Public_Virtual_Final_New_Void_EventData_0));

        ConfigurePatch(originalMethod, nameof(Prefix));
    }

    public static void ApplyPatch()
    {
        ApplyPatch<NetworkManagerOnEvent>();
    }

    public static void Prefix(EventData param_1)
    {
        if (param_1.Code == 12)
        {
            VRCPlayerApi sender = VRCUtils.GetVRCPlayerApiByID(param_1.Sender);
            if (sender != null) LastEvent12Time[sender] = Time.time;
        }
    }

    public static void OnUpdate()
    {
        if (!VRCUtils.IsInWorld())
            return;

        var crashedPlayers = LastEvent12Time
            .Where(entry => IsPlayerCrashed(entry.Key))
            .Select(entry => entry.Key)
            .ToList();

        foreach (var player in crashedPlayers)
        {
            LastEvent12Time.Remove(player);
            
            if (ModuleManager.GetModule<CrashDetector>().Enabled)
            {
                var message = $"Player {player.displayName} may have crashed!";
                Plugin.Log.LogInfo(message);
                NotificationManager.ShowNotification(message);
                AudioService.PlayAudio(AudioService.AudioClipType.PlayerCrashed);
            }
        }

        CleanupInvalidPlayers();
    }

    private static void CleanupInvalidPlayers()
    {
        var allPlayers = new HashSet<VRCPlayerApi>(VRCPlayerApi.AllPlayers.ToArray());
        List<VRCPlayerApi> invalidPlayers = LastEvent12Time.Keys
            .Where(player => player == null || !allPlayers.Contains(player))
            .ToList();

        foreach (VRCPlayerApi player in invalidPlayers) LastEvent12Time.Remove(player);
    }
}