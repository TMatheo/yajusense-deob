using System.Collections;
using UnityEngine;
using yajusense.Core;
using yajusense.Networking;

namespace yajusense.Modules.Photon;

public class Earrape : ModuleBase
{
	private const string Payload = "AAAAALuGOwD4fejAXKBS/jDkHruVxLHXjN9/0UI8AOI1tLBhaAT47sOGLLm2RF5yzbWWOOh+95t7rGxiUDhxWaCG3Q==";

	public Earrape() : base("Earrape", "Send earrape voice", ModuleCategory.Photon) { }

	public override void OnEnable()
	{
		CoroutineRunner.StartManagedCoroutine(SendEarrape());
	}

	private IEnumerator SendEarrape()
	{
		while (true)
		{
			if (!Enabled)
				yield break;

			EventSender.SendVoice(Payload);

			yield return new WaitForSeconds(0.1f);
		}
	}
}