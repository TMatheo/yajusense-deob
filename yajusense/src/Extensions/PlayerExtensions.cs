namespace yajusense.Extensions;

public static class PlayerExtensions
{
    public static PlayerNet_Internal GetPlayerNet(this Player_Internal player)
    {
        return player._playerNet;
    }
}