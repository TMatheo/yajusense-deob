using UnityEngine;
using yajusense.Core.Config;
using yajusense.Utils;

namespace yajusense.Modules.Movement;

public class Flight : BaseModule
{
    public Flight() : base("Flight", "Allows you to fly", ModuleCategory.Movement, KeyCode.F)
    {
    }

    [Config("Flight speed", "Flight speed", false, 1.0f, 50.0f)]
    public float Speed { get; set; } = 10.0f;

    public override void OnUpdate()
    {
        if (!VRCUtils.IsInWorld())
            return;

        var localPlayer = VRCUtils.GetLocalVRCPlayerApi();

        if (Input.GetKey(KeyCode.W))
            localPlayer.gameObject.transform.position +=
                localPlayer.gameObject.transform.forward * (Speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.S))
            localPlayer.gameObject.transform.position -=
                localPlayer.gameObject.transform.forward * (Speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.A))
            localPlayer.gameObject.transform.position -=
                localPlayer.gameObject.transform.right * (Speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.D))
            localPlayer.gameObject.transform.position +=
                localPlayer.gameObject.transform.right * (Speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Space))
            localPlayer.gameObject.transform.position += localPlayer.gameObject.transform.up * (Speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftShift))
            localPlayer.gameObject.transform.position -= localPlayer.gameObject.transform.up * (Speed * Time.deltaTime);

        localPlayer.SetVelocity(Vector3.zero);
    }

    public override void OnEnable()
    {
        if (!VRCUtils.IsInWorld())
            return;

        VRCUtils.GetLocalVRCPlayerApi().gameObject.GetComponent<CharacterController>().enabled = false;
    }

    public override void OnDisable()
    {
        if (!VRCUtils.IsInWorld())
            return;
        VRCUtils.GetLocalVRCPlayerApi().gameObject.GetComponent<CharacterController>().enabled = true;
    }
}