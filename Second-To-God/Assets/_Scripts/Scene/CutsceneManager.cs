using UnityEngine;
using UnityEngine.Playables;

public struct CutsceneData
{
	public PlayableAsset cutsceneAsset;
	public CutsceneBase cutsceneBehavior;
}

public class CutsceneManager : MonoBehaviour
{
	public static CutsceneManager Instance { get; private set; }
	public PlayableDirector cutsceneDirector;
	public CutsceneData[] cutscenes;
	private int currentCutsceneIndex;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	public void TriggerCutscene(int index)
	{
		if (index < 0 || index >= cutscenes.Length)
		{
			Debug.LogError("Cutscene index out of range.");
			return;
		}

		currentCutsceneIndex = index;
		cutsceneDirector.playableAsset = cutscenes[index].cutsceneAsset;
		cutscenes[index].cutsceneBehavior.StartCutscene();
		cutsceneDirector.Play();
	}

	public void TriggerEvent(string eventName)
	{
		var cutsceneBehavior = cutscenes[currentCutsceneIndex].cutsceneBehavior;
		if (cutsceneBehavior != null)
		{
			cutsceneBehavior.Trigger(eventName);
		}
		else
		{
			Debug.LogWarning("No CutsceneBase behavior found in the current playable asset.");
		}
	}
}
