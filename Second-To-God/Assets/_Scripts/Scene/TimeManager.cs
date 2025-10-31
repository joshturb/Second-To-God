using UnityEngine;
using UnityEngine.Rendering;

public class TimeManager : MonoBehaviour
{
	[SerializeField] private float dayColortemperature = 6500f;
	[SerializeField] private float dayLightIntensity = 1f;
	[SerializeField] private float nightColortemperature = 2000f;
	[SerializeField] private float nightLightIntensity = 0.1f;
	[SerializeField] private Gradient dayGradient = new();
	[SerializeField] private Gradient nightGradient = new();
	[SerializeField] private Material daySkybox;
	[SerializeField] private Material nightSkybox;
	[SerializeField] private Light directionalLight;

	[ContextMenu("Set Day ")]
	private void SetDay()
	{
		SetTime(true);
	}

	[ContextMenu("Set Night")]
	private void SetNight()
	{
		SetTime(false);
	}

	private void SetGradient(bool isDay)
	{
		if (!isDay)
		{
			RenderSettings.ambientMode = AmbientMode.Trilight;
			RenderSettings.ambientSkyColor = nightGradient.Evaluate(1f);
			RenderSettings.ambientEquatorColor = nightGradient.Evaluate(0.5f);
			RenderSettings.ambientGroundColor = nightGradient.Evaluate(0f);
			return;
		}

		RenderSettings.ambientMode = AmbientMode.Trilight;
		RenderSettings.ambientSkyColor = dayGradient.Evaluate(1f);
		RenderSettings.ambientEquatorColor = dayGradient.Evaluate(0.5f);
		RenderSettings.ambientGroundColor = dayGradient.Evaluate(0f);
	}

	private void SetDirectionalLight(bool isDay)
	{
		if (directionalLight == null)
			return;

		if (isDay)
		{
			directionalLight.colorTemperature = dayColortemperature;
			directionalLight.intensity = dayLightIntensity;
		}
		else
		{
			directionalLight.colorTemperature = nightColortemperature;
			directionalLight.intensity = nightLightIntensity;	
		}
	}

	public void SetTime(bool isDay)
	{
		if (isDay)
		{
			RenderSettings.skybox = daySkybox;
			SetDirectionalLight(true);
			SetGradient(true);
		}
		else
		{
			RenderSettings.skybox = nightSkybox;
			SetDirectionalLight(false);
			SetGradient(false);
		}
		DynamicGI.UpdateEnvironment();
	}
}
