using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace yajusense.Services;

public static class AssetBundleService
{
	private static readonly Dictionary<string, AssetBundle> LoadedBundles = new();

	public static AssetBundle LoadBundle(string bundleName)
	{
		string bundlePath = Path.Combine(Plugin.ClientDirectory, bundleName);

		if (!File.Exists(bundlePath))
		{
			Plugin.Log.LogError($"AssetBundle not found: {bundlePath}");
			return null;
		}

		if (LoadedBundles.TryGetValue(bundleName, out AssetBundle cachedBundle))
			return cachedBundle;

		AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
		if (bundle == null)
		{
			Plugin.Log.LogError($"Failed to load AssetBundle: {bundleName}");
			return null;
		}

		LoadedBundles[bundleName] = bundle;
		Plugin.Log.LogInfo($"AssetBundle loaded: {bundleName}");
		return bundle;
	}

	public static T LoadAsset<T>(string bundleName, string assetName) where T : Object
	{
		if (!LoadedBundles.TryGetValue(bundleName, out AssetBundle bundle))
		{
			Plugin.Log.LogError($"AssetBundle not loaded: {bundleName}");
			return null;
		}

		var asset = bundle.LoadAsset<T>(assetName);
		if (asset == null)
		{
			Plugin.Log.LogError($"Asset not found in bundle: {assetName}");
			return null;
		}

		return asset;
	}

	public static void UnloadBundle(string bundleName, bool unloadAllObjects = false)
	{
		if (LoadedBundles.TryGetValue(bundleName, out AssetBundle bundle))
		{
			bundle.Unload(unloadAllObjects);
			LoadedBundles.Remove(bundleName);
		}
	}

	public static void UnloadAllBundles(bool unloadAllObjects = false)
	{
		foreach (AssetBundle bundle in LoadedBundles.Values)
		{
			bundle.Unload(unloadAllObjects);
		}

		LoadedBundles.Clear();
	}
}