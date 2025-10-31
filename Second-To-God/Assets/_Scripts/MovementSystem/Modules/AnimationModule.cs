using UnityEngine;

[CreateAssetMenu(menuName = "Player Movement/Animation Module")]
public class AnimationModule : PlayerModule
{
	private Animator animator;
	private MovementModule movementModule;

	private bool isWalking;
	private bool isSprinting;

	public bool isLocked;
	public override bool IsLocked { get => isLocked; set => isLocked = value; }

	private bool isInitialized;
	public override bool IsInitialized { get => isInitialized; set => isInitialized = value; }

	public override void InitializeModule(FPCModule fPCModule)
	{
		IsLocked = false;
		isWalking = false;
		isSprinting = false;

		if (fPCModule.GetComponentInChildren<Animator>() is Animator animator)
		{
			this.animator = animator;
		}
		else
		{
			Debug.LogError("Animator component not found on the FPCModule.");
		}

		if (fPCModule.TryGetModule(out MovementModule movementModule))
		{
			this.movementModule = movementModule;
		}
		else
		{
			Debug.LogError("MovementModule not found on the FPCModule.");
		}
	}

	public override void OnModuleRemoved(FPCModule fPCModule)
	{
		isWalking = false;
		isSprinting = false;
	}

	public override void HandleInput(FPCModule fPCModule) { }

	public override void UpdateModule(FPCModule fPCModule)
	{
		if (movementModule.IsWalking)
		{
			isWalking = true;
		}
		else
		{
			isWalking = false;
		}

		if (movementModule.IsSprinting)
		{
			isSprinting = true;
		}
		else
		{
			isSprinting = false;
		}

		if (!movementModule.IsMoving)
		{
			isWalking = false;
			isSprinting = false;
		}

		animator.SetBool("IsWalking", isWalking);
		animator.SetBool("IsSprinting", isSprinting);
	}
}