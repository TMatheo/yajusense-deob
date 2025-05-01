using VRC.SDKBase;

namespace yajusense.Extensions;

public static class VRCPlayerApiExtensions
{
	public static Player_Internal GetPlayer(this VRCPlayerApi playerApi)
	{
		return playerApi.gameObject.GetComponent<Player_Internal>();
	}
}