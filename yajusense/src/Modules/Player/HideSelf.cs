using yajusense.Utils;

namespace yajusense.Modules.Player;

public class HideSelf : ModuleBase
{
    public HideSelf() : base("HideSelf", "Hides your avatar (only prevents from crashing when using one like mesh)", ModuleCategory.Player)
    {
    }

    public override void OnEnable()
    {
        if (!VRCUtils.IsInWorld())
            return;

        VRCUtils.GetLocalVRCPlayerApi().gameObject.transform.Find("ForwardDirection").gameObject.SetActive(false);
    }

    public override void OnDisable()
    {
        if (!VRCUtils.IsInWorld())
            return;

        VRCUtils.GetLocalVRCPlayerApi().gameObject.transform.Find("ForwardDirection").gameObject.SetActive(true);
    }
}