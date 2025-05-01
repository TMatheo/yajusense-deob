using yajusense.Utils.VRC;

namespace yajusense.Modules.Player;

public class Crasher : ModuleBase
{
    public Crasher() : base("Crasher", "Crashes the player", ModuleCategory.Player) { }

    public override void OnEnable()
    {
        if (PlayerUtils.IsInWorld())
        {
            ModuleManager.GetModule<HideSelf>().Enable();

            PlayerUtils.ChangeAvatar("avtr_187a312a-09a0-48a1-94ab-79786f8d981a");
        }
    }
}