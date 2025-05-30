using System;
using UnityEngine;
using yajusense.Modules;
using yajusense.Platform.VRC;
using yajusense.UI;

namespace yajusense.Core;

public class MainMonoBehaviour : MonoBehaviour
{
	public MainMonoBehaviour(IntPtr handle) : base(handle) { }

	private void Update()
	{
		ModuleManager.UpdateModules();
		PlayerTracker.Update();
	}

	private void OnGUI()
	{
		ModuleManager.RenderModules();
		UI.NotificationManager.OnGUI();
	}
}