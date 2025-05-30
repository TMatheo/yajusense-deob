using VRC;
using VRC.SDKBase;

namespace yajusense.Extensions;

public static class VRCPlayerApiExtensions
{
	public static Player GetPlayer(this VRCPlayerApi playerApi)
	{
		return playerApi.gameObject.GetComponent<Player>();
	}
}