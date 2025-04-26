using System.Collections.Generic;
using UnityEngine;

namespace yajusense.Core.Services;

public static class AudioService
{
    public enum AudioClipType
    {
        ModuleEnable,
        ModuleDisable,
        ClickUI,
        PlayerCrashed
    }

    private const string BundleName = "audiobundle";

    private static AudioSource _audioSource;

    private static readonly Dictionary<AudioClipType, string> AudioClipMap = new()
    {
        { AudioClipType.ModuleEnable, "module_enable" },
        { AudioClipType.ModuleDisable, "module_disable" },
        { AudioClipType.ClickUI, "click_ui" },
        { AudioClipType.PlayerCrashed, "player_crashed" }
    };

    public static void Initialize()
    {
        var go = new GameObject("AudioService");
        Object.DontDestroyOnLoad(go);
        _audioSource = go.AddComponent<AudioSource>();
        Plugin.Log.LogInfo("AudioService initialized");
    }

    public static void PlayAudio(AudioClipType clipType, float volume = 0.5f)
    {
        if (!AudioClipMap.TryGetValue(clipType, out string assetName))
        {
            Plugin.Log.LogError($"Audio clip mapping not found for type: {clipType}");
            return;
        }

        AssetBundle bundle = AssetBundleService.LoadBundle(BundleName);
        if (bundle == null) return;

        var clip = AssetBundleService.LoadAsset<AudioClip>(BundleName, assetName);
        if (clip != null) PlayAudioInternal(clip, volume);
    }

    private static void PlayAudioInternal(AudioClip clip, float volume)
    {
        if (clip == null) return;

        _audioSource.volume = volume;
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}