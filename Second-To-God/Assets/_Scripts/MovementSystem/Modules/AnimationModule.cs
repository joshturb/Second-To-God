using UnityEngine;
using UnityEngine.Animations.Rigging;

[CreateAssetMenu(menuName = "Player Movement/Animation Module")]
public class AnimationModule : PlayerModule
{
	private TwoBoneIKConstraint rightHandIK;
	private Animator animator;
	private MovementModule movementModule;
	private CrouchModule crouchModule;
	private PlayerInventory playerInventory;

	private bool isWalking;
	private bool isSprinting;
	private bool isCrouching;

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

		if (fPCModule.TryGetModule(out CrouchModule crouchModule))
		{
			this.crouchModule = crouchModule;
		}
		else
		{
			Debug.LogError("CrouchModule not found on the FPCModule.");
		}

		if (!fPCModule.TryGetComponent(out playerInventory))
		{
			Debug.LogError("PlayerInventory component not found on the FPCModule.");
		}
		playerInventory.OnSlotSelected += OnSlotSelected;
		playerInventory.OnInventorySlotChanged += OnInventorySlotChanged;

		rightHandIK = fPCModule.GetComponentInChildren<TwoBoneIKConstraint>();
		rightHandIK.weight = 0;
	}

	private void OnInventorySlotChanged(InventorySlot slot)
	{
		if (slot.IsEmpty())
		{
			rightHandIK.weight = 0;
		}
	}

	private void OnSlotSelected(int selectedSlot, int index)
	{
		if (playerInventory.CurrentEquippedItem == null)
		{
			rightHandIK.weight = 0;
			return;
		}
		ItemData item = playerInventory.inventorySlots[index].GetItemData();

		Transform equipped = playerInventory.CurrentEquippedItem.transform;
		// Use the equipped item's world position/rotation and apply the item-specific rotation offset
		rightHandIK.data.target.SetPositionAndRotation(equipped.position, equipped.rotation * Quaternion.Euler(item.ikRotation));
		rightHandIK.weight = 1;
	}

	public override void OnModuleRemoved(FPCModule fPCModule)
	{
		isWalking = false;
		isSprinting = false;
	}

	public override void HandleInput(FPCModule fPCModule)
	{
		if (playerInventory == null)
			return;

		if (playerInventory.CurrentEquippedItem == null)
			return;

		rightHandIK.data.target.position = playerInventory.CurrentEquippedItem.transform.position;
	}

	public override void UpdateModule(FPCModule fPCModule)
	{
		if (Hiding.IsPlayerHiding())
			return;

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

		if (crouchModule.isCrouching)
		{
			isCrouching = true;
		}
		else
		{
			isCrouching = false;
		}

		animator.SetBool("IsCrouching", isCrouching);
		animator.SetBool("IsWalking", isWalking);
		animator.SetBool("IsSprinting", isSprinting);
	}

	public void StopAnimations()
	{
		isCrouching = true;
		isWalking = false;
		isSprinting = false;
		animator.SetBool("IsCrouching", true);
		animator.SetBool("IsWalking", false);
		animator.SetBool("IsSprinting", false);
	}
}