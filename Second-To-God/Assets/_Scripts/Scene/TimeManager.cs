using UnityEngine;
using UnityEngine.Rendering;

public class TimeManager : MonoBehaviour
{
	[SerializeField] private Gradient dayGradient = new();
	[SerializeField] private Gradient nightGradient = new();

	[ContextMenu("Set Day Gradient")]
	private void SetDayGradient()
	{
		SetGradient(true);
	}

	[ContextMenu("Set Night Gradient")]
	private void SetNightGradient()
	{
		SetGradient(false);
	}

	private void SetGradient(bool isDay)
	{
		if (!isDay)
		{
			RenderSettings.ambientMode = AmbientMode.Trilight;
			RenderSettings.ambientSkyColor = nightGradient.Evaluate(1f);
			RenderSettings.ambientEquatorColor = nightGradient.Evaluate(0.5f);
			RenderSettings.ambientGroundColor = nightGradient.Evaluate(0f);
			DynamicGI.UpdateEnvironment();
			return;
		}

		RenderSettings.ambientMode = AmbientMode.Trilight;
		RenderSettings.ambientSkyColor = dayGradient.Evaluate(1f);
		RenderSettings.ambientEquatorColor = dayGradient.Evaluate(0.5f);
		RenderSettings.ambientGroundColor = dayGradient.Evaluate(0f);
		DynamicGI.UpdateEnvironment();
	}
}
