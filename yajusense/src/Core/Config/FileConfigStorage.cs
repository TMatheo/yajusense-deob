using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using yajusense.Utils;

namespace yajusense.Core.Config;

public class FileConfigStorage : IConfigStorage
{
	private const string ConfigFileName = "ModuleConfig.cock";
	private readonly string _configPath;
	private readonly JsonSerializerOptions _jsonOptions;

	public FileConfigStorage(JsonSerializerOptions jsonOptions)
	{
		_configPath = Path.Combine(Plugin.ClientDirectory, ConfigFileName);
		_jsonOptions = jsonOptions;
	}

	public void Save(JsonObject configData)
	{
		string json = JsonSerializer.Serialize(configData, _jsonOptions);
		File.WriteAllText(_configPath, json);
	}

	public JsonObject Load()
	{
		EnsureConfigFileExists();
		string json = File.ReadAllText(_configPath);

		if (string.IsNullOrWhiteSpace(json))
			return new JsonObject();

		JsonNode parsedNode = JsonNode.Parse(json);
		return parsedNode as JsonObject ?? throw new JsonException("Config file root is not a JSON object.");
	}

	public void CreateBackup(string suffix)
	{
		if (!File.Exists(_configPath))
			return;

		var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
		var backupPath = $"{_configPath}.{suffix}_{timestamp}.bak";
		File.Move(_configPath, backupPath);
	}

	public void EnsureConfigFileExists()
	{
		if (!File.Exists(_configPath))
		{
			string directory = Path.GetDirectoryName(_configPath)!;
			FileUtils.EnsureDirectoryExists(directory);

			File.WriteAllText(_configPath, "{}");
		}
	}
}