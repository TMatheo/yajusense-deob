using UnityEngine;
using Yjsnpi.Core.Config;
using Yjsnpi.Utilities;

namespace Yjsnpi.Modules.Visual;

public class Flight : BaseModule
{
    [Config("Flight speed", "Flight speed", 1.0f, 50.0f)]
    private float Speed { get; } = 5.0f;

    public Flight() : base("Flight", "Allows you to fly", ModuleType.Movement, KeyCode.F) {}

    public override void OnUpdate()
    {
        if (!VRCUtility.IsInWorld()) return;
        
        var localPlayer = VRCUtility.GetLocalVRCPlayerApi();
        
        if (Input.GetKeyDown(KeyCode.W))
            localPlayer.gameObject.transform.position += Vector3.up * (Speed * Time.deltaTime);
        
        if (Input.GetKeyDown(KeyCode.S))
            localPlayer.gameObject.transform.position -= Vector3.up * (Speed * Time.deltaTime);
        
        if (Input.GetKeyDown(KeyCode.A))
            localPlayer.gameObject.transform.position -= Vector3.right * (Speed * Time.deltaTime);
        
        if (Input.GetKeyDown(KeyCode.D))
            localPlayer.gameObject.transform.position += Vector3.right * (Speed * Time.deltaTime);
        
        localPlayer.SetVelocity(Vector3.zero);
    }

    public override void OnEnable()
    {
        if (!VRCUtility.IsInWorld()) return;
        
        VRCUtility.GetLocalVRCPlayerApi().gameObject.GetComponent<CharacterController>().enabled = false;
    }

    public override void OnDisable()
    {
        if (!VRCUtility.IsInWorld()) return;
        
        VRCUtility.GetLocalVRCPlayerApi().gameObject.GetComponent<CharacterController>().enabled = true;
    }
}