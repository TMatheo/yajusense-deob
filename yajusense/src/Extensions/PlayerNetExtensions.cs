namespace yajusense.Extensions;

public static class PlayerNetExtensions
{
    public static int GetPhotonNumber(this PlayerNet_Internal playerNet)
    {
        if (playerNet == null) return -114514;
        
        return playerNet.prop_Int32_0;
    }
}