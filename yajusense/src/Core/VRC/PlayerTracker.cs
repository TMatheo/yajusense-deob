using System.Collections.Generic;
using System.Linq;
using VRC.SDKBase;
using yajusense.Modules;
using yajusense.Utils.VRC;

namespace yajusense.Core.VRC;

public static class PlayerTracker
{
    private static Dictionary<int, VRCPlayerApi> _currentPlayers = new();
    private static Dictionary<int, VRCPlayerApi> _previousPlayers = new();

    public static void Update()
    {
        if (!PlayerUtils.IsInWorld())
            return;

        _previousPlayers = new Dictionary<int, VRCPlayerApi>(_currentPlayers);
        _currentPlayers = VRCPlayerApi.AllPlayers.ToArray().Where(p => p != null).ToDictionary(p => p.playerId, p => p);

        List<VRCPlayerApi> joinedPlayers = _currentPlayers.Where(kv => !_previousPlayers.ContainsKey(kv.Key)).Select(kv => kv.Value).ToList();

        List<VRCPlayerApi> leftPlayers = _previousPlayers.Where(kv => !_currentPlayers.ContainsKey(kv.Key)).Select(kv => kv.Value).ToList();

        if (joinedPlayers.Any() || leftPlayers.Any())
            ModuleManager.NotifyPlayerChanges(joinedPlayers, leftPlayers);
    }
}