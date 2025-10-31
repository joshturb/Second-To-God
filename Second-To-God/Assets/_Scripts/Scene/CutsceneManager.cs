using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
	public static CutsceneManager Instance { get; private set; }
	public PlayableDirector cutsceneDirector;
	public PlayableAsset[] cutscenes;

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

		cutsceneDirector.playableAsset = cutscenes[index];
		cutsceneDirector.Play();
	}
}
