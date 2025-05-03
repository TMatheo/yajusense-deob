namespace yajusense.Utils.VRC;

public static class NetworkingUtils
{
	public static int GetServerTimeMS()
	{
		return global::VRC.SDKBase.Networking.GetServerTimeInMilliseconds();
	}
}