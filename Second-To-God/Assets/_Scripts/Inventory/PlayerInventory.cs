using System;
using UnityEngine;

public class PlayerInventory : InventoryBase, IGUI
{
    public int SelectedSlot = 0;

	public Transform holdTransform;
	public Transform dropTransform;
	public GameObject CurrentEquippedItem;
    private int lastSelectedSlot = -1;
	public event Action<int,int> OnSlotSelected;

	private void Start()
	{
		Initialize();
	}

	public void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
		if (InputHandler.Instance.playerActions._1.WasPressedThisFrame())
		{
			SelectSlot(0);
		}
		if (InputHandler.Instance.playerActions._2.WasPressedThisFrame())
		{
			SelectSlot(1);
		}
		if (InputHandler.Instance.playerActions._3.WasPressedThisFrame())
		{
			SelectSlot(2);
		}
        if (InputHandler.Instance.playerActions.Drop.WasPressedThisFrame() && !inventorySlots[SelectedSlot].IsEmpty()) 
        {
            DropItem();
        }
        if (InputHandler.Instance.playerActions.RightBumper.WasPressedThisFrame())
        {
            BumperNavigation(true);
        }
        if (InputHandler.Instance.playerActions.LeftBumper.WasPressedThisFrame())
        {
            BumperNavigation(false);
        }
		if (InputHandler.Instance.playerActions.Attack.WasPressedThisFrame())
		{
			if (CurrentEquippedItem == null || !CurrentEquippedItem.TryGetComponent(out IUsable usable)) 
				return;

			usable.Use();
		}

        ScrollWheelNavigation(InputHandler.Instance.playerActions.Scroll.ReadValue<Vector2>().y);
    }


	private void ScrollWheelNavigation(float value)
    {
        if (value == 0) return;
        
        int slotOffset = value > 0 ? -1 : 1;
        int newSlot = (SelectedSlot + slotOffset + 5) % 5; // Ensures cyclic behavior

        SelectSlot(newSlot);
    }

    private void BumperNavigation(bool value)
    {
        int newSlot = value ? (SelectedSlot == 0 ? InventorySize : SelectedSlot - 1): 
                              (SelectedSlot == InventorySize ? 0 : SelectedSlot + 1);
        SelectSlot(newSlot);
    }

    private void SelectSlot(int slotIndex)
	{
		if (slotIndex < 0 || slotIndex >= InventorySize)
			return;

        if (slotIndex == lastSelectedSlot && CurrentEquippedItem != null)
        {
            HolsterItem();
            lastSelectedSlot = -1;
            return;
        }

		HolsterItem();
        EquipItem(slotIndex);
		
        OnSlotSelected?.Invoke(SelectedSlot, slotIndex);
		SelectedSlot = slotIndex;
	
        lastSelectedSlot = slotIndex;
    }


    // Local (singleplayer) equip implementation
    private void EquipItem(int slotIndex)
    {
        InventorySlot slot = inventorySlots[slotIndex];

        if (CurrentEquippedItem != null)
            Destroy(CurrentEquippedItem);

        if (slot.IsEmpty())
            return;

        ItemData itemData = ItemDatabase.Instance.GetItemByID(slot.GetItemID());

        var spawnedObj = Instantiate(itemData.holdingPrefab);
		spawnedObj.transform.SetParent(holdTransform);
		spawnedObj.transform.position = holdTransform.position;
		spawnedObj.transform.localRotation = Quaternion.Euler(itemData.holdRotation);
		
        CurrentEquippedItem = spawnedObj;
        CurrentEquippedItem.name = spawnedObj.transform.name;
    }

    public void HolsterItem()
    {
        if (CurrentEquippedItem != null)
        {
            Destroy(CurrentEquippedItem);
        }
        CurrentEquippedItem = null;
    }

    // Local drop implementation
    public void DropItem()
    {
        if (CurrentEquippedItem == null)
            return;

        LayerMask delayedInteractableMask = LayerMask.GetMask("IDelayedInteractable");
        if (!Physics.Raycast(dropTransform.position, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, ~delayedInteractableMask))
            return;

        ItemData itemData = GetHeldItemData();
        
        HolsterItem();

        Instantiate(itemData.groundPrefab, hitInfo.point + new Vector3(0, 0.1f, 0), Quaternion.Euler(itemData.groundRotation));

        RemoveItem(SelectedSlot);
    }

    public ItemData GetHeldItemData()
    {
        if (CurrentEquippedItem == null)
            return null;

        if (inventorySlots[SelectedSlot].IsEmpty())
            return null;

        return ItemDatabase.Instance.GetItemByID(inventorySlots[SelectedSlot].GetItemID());
    }

	public bool IsHolding(ItemTypeId itemTypeID)
	{
		if (CurrentEquippedItem == null)
			return false;

		ItemData data = GetHeldItemData();

		if (data == null || data != null && data.itemTypeId != itemTypeID)
			return false;

		return true;
	}
	
	public bool IsInventoryFull()
	{
		foreach (var slot in inventorySlots)
		{
			if (slot.IsEmpty())
				return false;
		}
		return true;
	}

}
