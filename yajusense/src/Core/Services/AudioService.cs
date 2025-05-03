using System.Collections.Generic;
using UnityEngine;
using yajusense.Modules;

namespace yajusense.Core.Services;

public static class AudioService
{
	public enum AudioClipType
	{
		ModuleEnable,
		ModuleDisable,
		ClickUI,
		PlayerNotification,
		Teleport,
	}

	private const string BundleName = "assetbundle";

	private static AudioSource _audioSource;

	private static readonly Dictionary<AudioClipType, string> AudioClipMap = new()
	{
		{ AudioClipType.ModuleEnable, "module_enable" },
		{ AudioClipType.ModuleDisable, "module_disable" },
		{ AudioClipType.ClickUI, "click_ui" },
		{ AudioClipType.PlayerNotification, "player_notification" },
		{ AudioClipType.Teleport, "teleport" },
	};

	public static void Initialize()
	{
		var go = new GameObject("AudioService")
		{
			hideFlags = HideFlags.HideAndDontSave,
		};
		_audioSource = go.AddComponent<AudioSource>();
		Plugin.Log.LogInfo("AudioService initialized");
	}

	public static void PlayAudio(AudioClipType clipType)
	{
		if (!AudioClipMap.TryGetValue(clipType, out string assetName))
		{
			Plugin.Log.LogError($"Audio clip mapping not found for type: {clipType}");
			return;
		}

		AssetBundle bundle = AssetBundleService.LoadBundle(BundleName);
		if (bundle == null)
			return;

		var clip = AssetBundleService.LoadAsset<AudioClip>(BundleName, assetName);
		if (clip != null)
			PlayAudioInternal(clip, ModuleManager.ClientSettings.SoundEffectsVolume);
	}

	private static void PlayAudioInternal(AudioClip clip, float volume)
	{
		if (clip == null)
			return;

		_audioSource.volume = volume;
		_audioSource.clip = clip;
		_audioSource.Play();
	}
}