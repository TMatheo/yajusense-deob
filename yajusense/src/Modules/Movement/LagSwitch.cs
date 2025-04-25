using yajusense.Patches;

namespace yajusense.Modules.Movement;

public class LagSwitch : ModuleBase
{
    public LagSwitch() : base("LagSwitch", "Suspends sending all movement events to server while enabled", ModuleCategory.Movement)
    {
    }

    public override void OnEnable()
    {
        OpRaiseEvent.ShouldSendE12 = false;
    }

    public override void OnDisable()
    {
        OpRaiseEvent.ShouldSendE12 = true;
    }
}