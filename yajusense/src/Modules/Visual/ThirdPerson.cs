using UnityEngine;

namespace yajusense.Modules.Visual;

public class ThirdPerson : ModuleBase
{
	// 定数
	private const float MaxZoomOffset = 5f;
	private const float MinZoomOffset = 0.5f;
	private const float ZoomLerpSpeed = 10f;

	// ズーム関連
	private float _currentZoomOffset = 2f;

	private Camera _referenceCameraComponent;

	// 参照カメラ関連
	private GameObject _referenceCameraGameObject;
	private Transform _referenceCameraTransform;
	private float _targetZoomOffset = 2f;
	private Camera _thirdPersonCameraComponent;

	// サードパーソンカメラ関連
	private GameObject _thirdPersonCameraGameObject;
	private Transform _thirdPersonCameraTransform;

	public ThirdPerson() : base("ThirdPerson", "Back view camera", ModuleCategory.Visual, KeyCode.F5) { }

    /// <summary>
    ///     サードパーソンカメラコンポーネントを取得
    /// </summary>
    public Camera GetCamera()
	{
		return _thirdPersonCameraComponent;
	}

	public override void OnEnable()
	{
		if (InitializeCameras())
			EnableThirdPersonCamera();
	}

	public override void OnDisable()
	{
		DisableThirdPersonCamera();
	}

	private void ClearCache()
	{
		_referenceCameraGameObject = null;
		_referenceCameraComponent = null;
		_referenceCameraTransform = null;

		_thirdPersonCameraGameObject = null;
		_thirdPersonCameraComponent = null;
		_thirdPersonCameraTransform = null;
	}

	private bool InitializeCameras()
	{
		// 参照カメラの初期化
		if (_referenceCameraGameObject == null)
		{
			_referenceCameraGameObject = GameObject.Find("SteamCamera/[CameraRig]/Neck/Camera");
			if (_referenceCameraGameObject == null)
			{
				ClearCache();
				return false;
			}

			_referenceCameraTransform = _referenceCameraGameObject.transform;
			_referenceCameraComponent = _referenceCameraGameObject.GetComponent<Camera>();
		}

		// サードパーソンカメラの初期化
		if (_thirdPersonCameraGameObject == null && _referenceCameraTransform != null)
		{
			_thirdPersonCameraGameObject = CreateCameraObject("ThirdPersonCamera", _referenceCameraTransform);
			if (_thirdPersonCameraGameObject != null)
			{
				_thirdPersonCameraComponent = _thirdPersonCameraGameObject.GetComponent<Camera>();
				_thirdPersonCameraTransform = _thirdPersonCameraGameObject.transform;
			}
		}

		// 初期化チェック
		if (_referenceCameraComponent == null || _thirdPersonCameraGameObject == null || _thirdPersonCameraComponent == null || _thirdPersonCameraTransform == null)
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

		// 物理挙動を無効化
		var rigidbody = cameraObj.AddComponent<Rigidbody>();
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;

		// カメラ設定
		var camera = cameraObj.AddComponent<Camera>();
		camera.nearClipPlane /= 4;
		camera.enabled = false;

		return cameraObj;
	}

	private void EnableThirdPersonCamera()
	{
		if (_thirdPersonCameraComponent == null || _referenceCameraComponent == null)
		{
			if (!InitializeCameras())
				return;
		}

		_referenceCameraComponent.enabled = false;
		_thirdPersonCameraComponent.enabled = true;
		UpdateCameraPosition();
	}

	private void DisableThirdPersonCamera()
	{
		if (_thirdPersonCameraComponent != null)
			_thirdPersonCameraComponent.enabled = false;

		if (_referenceCameraComponent != null)
			_referenceCameraComponent.enabled = true;
	}

	private void UpdateCameraPosition()
	{
		if (_referenceCameraTransform == null || _thirdPersonCameraTransform == null)
			return;

		_thirdPersonCameraTransform.position = _referenceCameraTransform.position - _referenceCameraTransform.forward * _currentZoomOffset;
	}

	public override void OnUpdate()
	{
		if (_referenceCameraGameObject == null || _thirdPersonCameraGameObject == null)
		{
			if (!InitializeCameras())
				return;
		}

		// マウスホイールでズーム調整
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (Mathf.Abs(scroll) > 0.01f)
			_targetZoomOffset = Mathf.Clamp(_targetZoomOffset - scroll, MinZoomOffset, MaxZoomOffset);

		// スムーズなズーム処理
		_currentZoomOffset = Mathf.Lerp(_currentZoomOffset, _targetZoomOffset, Time.deltaTime * ZoomLerpSpeed);
		UpdateCameraPosition();
	}
}