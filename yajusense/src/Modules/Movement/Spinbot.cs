using System.Collections;
using UnityEngine;
using yajusense.Core;
using yajusense.Core.Config;
using yajusense.Networking;
using yajusense.Utils;

namespace yajusense.Modules.Movement;

public class Spinbot : BaseModule
{
    private Coroutine _coroutine;
    
    [Config("Rotation speed", "Rotation speed", false, 1.0f, 50.0f)]
    public float RotationSpeed { get; set; } = 25.0f;
    
    public Spinbot() : base("Spinbot", "Spins the player (server-sided)", ModuleCategory.Movement)
    {
    }

    public override void OnEnable()
    {
        if (!VRCUtils.IsInWorld())
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
            if (!VRCUtils.IsInWorld())
                yield break;
            
            var spinRotation = Quaternion.Euler(0f, Time.time * RotationSpeed % 360f, 0f);
            
            EventSender.SendMovementEvent(VRCUtils.GetLocalVRCPlayerApi().gameObject.transform.position, spinRotation);
            yield return new WaitForSeconds(0.1f);
        }
    }
}