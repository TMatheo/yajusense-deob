using yajusense.Patches;

namespace yajusense.Modules.Movement;

public class LagSwitch : BaseModule
{
    public LagSwitch() : base("LagSwitch", "Suspends sending all movement events to server while enabled", ModuleCategory.Movement)
    {
    }

    public override void OnEnable()
    {
        OpRaiseEventPatch.ShouldSendE12 = false;
    }

    public override void OnDisable()
    {
        OpRaiseEventPatch.ShouldSendE12 = true;
    }
}