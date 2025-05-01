using UnityEngine;

namespace yajusense.Modules.Visual;

public class ThirdPerson : ModuleBase
{
    private const float MaxZoomOffset = 5f;
    private const float MinZoomOffset = 0.5f;
    private const float ZoomLerpSpeed = 10f;
    private Camera _cachedReferenceCameraComponent;

    private GameObject _cachedReferenceCameraObject;
    private Transform _referenceCameraTransform;

    private float _targetZoomOffset = 2f;
    private Camera _tpCameraBackComponent;
    private GameObject _tpCameraBackObject;
    private Transform _tpCameraBackTransform;
    private float _zoomOffset = 2f;

    public ThirdPerson() : base("ThirdPerson", "Back view camera", ModuleCategory.Visual, KeyCode.F5) { }


    public override void OnEnable()
    {
        if (InitializeCameras())
            EnableBackCamera();
    }

    public override void OnDisable()
    {
        DisableBackCamera();
    }

    private void ClearCache()
    {
        _cachedReferenceCameraObject = null;
        _cachedReferenceCameraComponent = null;
        _tpCameraBackObject = null;
        _tpCameraBackComponent = null;
        _tpCameraBackTransform = null;
        _referenceCameraTransform = null;
    }

    private bool InitializeCameras()
    {
        if (_cachedReferenceCameraObject == null)
            _cachedReferenceCameraObject = GameObject.Find("SteamCamera/[CameraRig]/Neck/Camera");

        if (_cachedReferenceCameraObject == null)
        {
            ClearCache();
            return false;
        }

        if (_referenceCameraTransform == null)
            _referenceCameraTransform = _cachedReferenceCameraObject.transform;

        if (_cachedReferenceCameraComponent == null)
            _cachedReferenceCameraComponent = _cachedReferenceCameraObject.GetComponent<Camera>();

        if (_tpCameraBackObject == null && _referenceCameraTransform != null)
        {
            _tpCameraBackObject = CreateCameraObject("TPCameraBack", _referenceCameraTransform);
            if (_tpCameraBackObject != null)
            {
                _tpCameraBackComponent = _tpCameraBackObject.GetComponent<Camera>();
                _tpCameraBackTransform = _tpCameraBackObject.transform;
            }
        }

        if (_cachedReferenceCameraComponent == null || _tpCameraBackObject == null || _tpCameraBackComponent == null || _tpCameraBackTransform == null)
        {
            ClearCache();
            Plugin.Log.LogError("[ThirdPerson] Failed to find/create necessary camera components.");
            return false;
        }


        UpdateCameraPosition();
        return true;
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
        // Use cached components
        if (_tpCameraBackComponent == null || _cachedReferenceCameraComponent == null)
        {
            // Attempt reinitialization if components are missing
            if (!InitializeCameras())
                return;
        }

        if (_cachedReferenceCameraComponent != null)
            _cachedReferenceCameraComponent.enabled = false;

        if (_tpCameraBackComponent != null)
            _tpCameraBackComponent.enabled = true;

        UpdateCameraPosition();
    }

    private void DisableBackCamera()
    {
        // Use cached components
        // No need to check world state here, just disable/enable based on cache
        if (_tpCameraBackComponent != null)
            _tpCameraBackComponent.enabled = false;

        if (_cachedReferenceCameraComponent != null)
            _cachedReferenceCameraComponent.enabled = true;
    }


    private void UpdateCameraPosition()
    {
        // Use cached transforms
        if (_referenceCameraTransform == null || _tpCameraBackTransform == null)
            return;

        _tpCameraBackTransform.position = _referenceCameraTransform.position - _referenceCameraTransform.forward * _zoomOffset;
    }

    public override void OnUpdate()
    {
        if (_cachedReferenceCameraObject == null || _tpCameraBackObject == null)
        {
            if (!InitializeCameras())
                return;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
            _targetZoomOffset = Mathf.Clamp(_targetZoomOffset - scroll, MinZoomOffset, MaxZoomOffset);

        _zoomOffset = Mathf.Lerp(_zoomOffset, _targetZoomOffset, Time.deltaTime * ZoomLerpSpeed);
        UpdateCameraPosition();
    }
}