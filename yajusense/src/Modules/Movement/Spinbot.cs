using System.Collections;
using UnityEngine;
using yajusense.Core;
using yajusense.Core.Config;
using yajusense.Networking;
using yajusense.Utils;
using yajusense.Utils.VRC;

namespace yajusense.Modules.Movement;

public class Spinbot : ModuleBase
{
    private Coroutine _coroutine;

    public Spinbot() : base("Spinbot", "Player goes crazy (server-sided)", ModuleCategory.Movement)
    {
    }

    [Config("Rotation speed", "Rotation speed", false, 1.0f, 100.0f)]
    public float RotationSpeed { get; set; } = 50.0f;

    public Quaternion Rotation { get; private set; } = Quaternion.identity;

    public override void OnUpdate()
    {
        Rotation = Quaternion.Euler(Time.time * RotationSpeed % 360f, Time.time * RotationSpeed % 360f, Time.time * RotationSpeed % 360f);
    }

    public override void OnEnable()
    {
        if (!PlayerUtils.IsInWorld())
            return;

        _coroutine = CoroutineRunner.StartManagedCoroutine(SendSpinEvent());
    }

    public override void OnDisable()
    {
        if (_coroutine != null)
            CoroutineRunner.StopManagedCoroutine(_coroutine);
    }

    private IEnumerator SendSpinEvent()
    {
        while (Enabled)
        {
            if (!Utils.VRC.PlayerUtils.IsInWorld())
                yield break;

            EventSender.SendMovementEvent(Utils.VRC.PlayerUtils.GetLocalVRCPlayerApi().gameObject.transform.position, Rotation);
            yield return new WaitForSeconds(0.1f);
        }
    }
}