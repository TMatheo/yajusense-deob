using UnityEngine;
using VRC.SDKBase;
using yajusense.Core.Config;
using yajusense.Platform.VRC;

namespace yajusense.Modules.Movement;

public class Flight : ModuleBase
{
	private CharacterController _characterController;
	private VRCPlayerApi _localPlayerApi;
	private Transform _playerTransform;

	public Flight() : base("Flight", "Allows you to fly", ModuleCategory.Movement, KeyCode.F) { }

	[Config("Flight speed", "Flight speed", false, 1.0f, 50.0f)]
	public float Speed { get; set; } = 10.0f;

	private bool EnsurePlayerComponents()
	{
		_localPlayerApi = VRCHelper.GetLocalVRCPlayerApi();
		if (_localPlayerApi == null)
		{
			_playerTransform = null;
			_characterController = null;
			return false;
		}

		if (_playerTransform == null)
			_playerTransform = _localPlayerApi.gameObject?.transform;

		if (_characterController == null)
			_characterController = _localPlayerApi.gameObject?.GetComponent<CharacterController>();

		return _playerTransform != null && _characterController != null;
	}


	public override void OnUpdate()
	{
		if (!EnsurePlayerComponents())
			return;

		if (Input.GetKey(KeyCode.W))
			_playerTransform.position += _playerTransform.forward * (Speed * Time.deltaTime);

		if (Input.GetKey(KeyCode.S))
			_playerTransform.position -= _playerTransform.forward * (Speed * Time.deltaTime);

		if (Input.GetKey(KeyCode.A))
			_playerTransform.position -= _playerTransform.right * (Speed * Time.deltaTime);

		if (Input.GetKey(KeyCode.D))
			_playerTransform.position += _playerTransform.right * (Speed * Time.deltaTime);

		if (Input.GetKey(KeyCode.Space))
			_playerTransform.position += _playerTransform.up * (Speed * Time.deltaTime);

		if (Input.GetKey(KeyCode.LeftShift))
			_playerTransform.position -= _playerTransform.up * (Speed * Time.deltaTime);

		_localPlayerApi.SetVelocity(Vector3.zero);
	}

	public override void OnEnable()
	{
		if (!EnsurePlayerComponents())
			return;

		if (_characterController != null)
			_characterController.enabled = false;
	}

	public override void OnDisable()
	{
		if (!EnsurePlayerComponents())
			return;

		if (_characterController != null)
			_characterController.enabled = true;
	}
}