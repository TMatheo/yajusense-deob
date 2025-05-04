using System.Text.Json.Nodes;

namespace yajusense.Core.Config;

public interface IConfigStorage
{
	void Save(JsonObject configData);
	JsonObject Load();
	void CreateBackup(string suffix);
}