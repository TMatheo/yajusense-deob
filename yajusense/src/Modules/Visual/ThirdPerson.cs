using UnityEngine;
using yajusense.Utils;

namespace yajusense.Modules.Visual;

public class ThirdPerson : ModuleBase
{
    private const float MaxZoomOffset = 5f;
    private const float MinZoomOffset = 0.5f;
    private const float ZoomLerpSpeed = 10f;
    private GameObject _referenceCamera;
    private float _targetZoomOffset = 2f;
    private GameObject _tpCameraBack;

    private float _zoomOffset = 2f;

    public ThirdPerson() : base("ThirdPerson", "Back view camera", ModuleCategory.Visual, KeyCode.F5) { }

    public override void OnEnable()
    {
        if (!VRCUtils.IsInWorld())
            return;

        InitializeCameras();
        EnableBackCamera();
    }

    public override void OnDisable()
    {
        if (!VRCUtils.IsInWorld())
            return;

        DisableBackCamera();
    }

    private void InitializeCameras()
    {
        if (_referenceCamera != null) return;

        _referenceCamera = GameObject.Find("SteamCamera/[CameraRig]/Neck/Camera");
        if (_referenceCamera == null) return;

        _tpCameraBack = CreateCameraObject("TPCameraBack", _referenceCamera.transform);
        UpdateCameraPosition();
    }

    private GameObject CreateCameraObject(string name, Transform parent)
    {
        var cameraObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Object.Destroy(cameraObj.GetComponent<MeshRenderer>());
        Object.Destroy(cameraObj.GetComponent<Collider>());

        cameraObj.name = name;
        cameraObj.transform.SetParent(parent, false);

        var rb = cameraObj.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var camera = cameraObj.AddComponent<Camera>();
        camera.fieldOfView = 75f;
        camera.nearClipPlane /= 4;
        camera.enabled = false;

        return cameraObj;
    }

    private void EnableBackCamera()
    {
        if (_tpCameraBack == null || _referenceCamera == null)
            return;

        if (_referenceCamera.GetComponent<Camera>() != null)
            _referenceCamera.GetComponent<Camera>().enabled = false;

        _tpCameraBack.GetComponent<Camera>().enabled = true;
        UpdateCameraPosition();
    }

    private void DisableBackCamera()
    {
        if (_tpCameraBack == null || _referenceCamera == null)
            return;

        _tpCameraBack.GetComponent<Camera>().enabled = false;

        if (_referenceCamera.GetComponent<Camera>() != null)
            _referenceCamera.GetComponent<Camera>().enabled = true;
    }

    private void UpdateCameraPosition()
    {
        if (_referenceCamera == null || _tpCameraBack == null) return;

        _tpCameraBack.transform.position =
            _referenceCamera.transform.position - _referenceCamera.transform.forward * _zoomOffset;
    }

    public override void OnUpdate()
    {
        if (!VRCUtils.IsInWorld())
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
            _targetZoomOffset = Mathf.Clamp(_targetZoomOffset + scroll, MinZoomOffset, MaxZoomOffset);

        _zoomOffset = Mathf.Lerp(_zoomOffset, _targetZoomOffset, Time.deltaTime * ZoomLerpSpeed);
        UpdateCameraPosition();
    }
}