using UnityEngine;

public abstract class CutsceneBase : MonoBehaviour
{
	public virtual void StartCutscene()
	{
		// Override in derived classes to implement cutscene logic
	}

	public virtual void UpdateCutscene()
	{
		// Override in derived classes to implement cutscene logic
	}

	public virtual void Trigger(string eventName)
	{
		
	}

	public virtual void EndCutscene()
	{
		// Override in derived classes to implement cutscene logic
	}
}
