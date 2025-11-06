using UnityEngine;

public class Hiding : MonoBehaviour, IInteractable, IHoverText
{
	[SerializeField] private Vector3 hidingOffset;
	public string HoverText => "" + (isHiding ? "(Space) To Exit" : "Hide");
	private Vector3 originalPlayerPosition;
	private Transform playerTransform;
	private bool isHiding = false;

	private void LateUpdate()
	{
		if (!isHiding)
			return;

		if (InputHandler.Instance.playerActions.Jump.WasPressedThisFrame() && isHiding)
		{
			UnhidePlayer(playerTransform);
		}
	}

	public void Interact(RaycastHit hit, Transform player)
	{
		if (!isHiding)
		{
			playerTransform = player;
			originalPlayerPosition = player.position;
			HidePlayer(player);
		}
	}

	void HidePlayer(Transform player)
	{
		if (!player.TryGetComponent(out FPCModule fpcModule))
		{
			Debug.LogWarning("FPCModule not found on Player object.");
			return;
		}

		if (!fpcModule.TryGetModule(out JumpModule jumpModule))
		{
			Debug.LogWarning("JumpModule not found on FPCModule.");
			return;
		}

		if (!fpcModule.TryGetModule(out MovementModule movementModule))
		{
			Debug.LogWarning("MovementModule not found on FPCModule.");
			return;
		}

		if (!fpcModule.TryGetModule(out CrouchModule crouchModule))
		{
			Debug.LogWarning("CrouchModule not found on FPCModule.");
			return;
		}

		if (!fpcModule.TryGetModule(out AnimationModule animationModule))
		{
			Debug.LogWarning("AnimationModule not found on FPCModule.");
			return;
		}

		if (!fpcModule.TryGetComponent(out Interaction interaction))
		{
			Debug.LogWarning("Interaction component not found on FPCModule.");
			return;
		}
		
		isHiding = true;

		animationModule.StopAnimations();

		interaction.SetHoverText(HoverText);
		interaction.LockHoverText();
		crouchModule.SetCrouchState(true);

		jumpModule.IsLocked = true;
		movementModule.IsLocked = true;
		crouchModule.IsLocked = true;
	
		fpcModule.characterController.enabled = false;
		player.transform.position = transform.position + hidingOffset;
	}

	void UnhidePlayer(Transform player)
	{
		if (!player.TryGetComponent(out FPCModule fpcModule))
		{
			Debug.LogWarning("FPCModule not found on Player object.");
			return;
		}

		if (!fpcModule.TryGetModule(out JumpModule jumpModule))
		{
			Debug.LogWarning("JumpModule not found on FPCModule.");
			return;
		}

		if (!fpcModule.TryGetModule(out MovementModule movementModule))
		{
			Debug.LogWarning("MovementModule not found on FPCModule.");
			return;
		}

		if (!fpcModule.TryGetModule(out CrouchModule crouchModule))
		{
			Debug.LogWarning("CrouchModule not found on FPCModule.");
			return;
		}

		if (!fpcModule.TryGetComponent(out Interaction interaction))
		{
			Debug.LogWarning("Interaction component not found on FPCModule.");
			return;
		}

		interaction.UnlockHoverText();

		crouchModule.SetCrouchState(false);
		fpcModule.movement = Vector3.zero;
		player.transform.position = originalPlayerPosition;
		fpcModule.characterController.enabled = true;
		jumpModule.IsLocked = false;
		movementModule.IsLocked = false;
		crouchModule.IsLocked = false;
		isHiding = false;
	}

	public static bool IsPlayerHiding()
	{
		Hiding[] hidingInstances = FindObjectsByType<Hiding>(FindObjectsSortMode.None);
		foreach (Hiding hiding in hidingInstances)
		{
			if (hiding.isHiding)
			{
				return true;
			}
		}
		return false;
	}

	public static Hiding GetPlayerHidingSpot()
	{
		Hiding[] hidingInstances = FindObjectsByType<Hiding>(FindObjectsSortMode.None);
		foreach (Hiding hiding in hidingInstances)
		{
			if (hiding.isHiding)
			{
				return hiding;
			}
		}
		return null;
	}
}
