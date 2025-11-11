using System;
using UnityEngine;

[Serializable]
public enum InventoryType
{
	None,
	Player,
	Ritual,
	Machine,
}

[Serializable]
public enum SlotState
{
	Locked,
	Input,
	Output,
	Both,
}

[Serializable]
public struct InventorySlot
{
    [SerializeField] private ItemTypeId itemTypeId;
	[SerializeField] private InventoryType inventoryType;
    [SerializeField] private int slotID;
	[SerializeField] private SlotState slotState;

    public InventorySlot(InventoryType inventoryType, int slotID, SlotState slotState = SlotState.Both)
    {
        itemTypeId = ItemTypeId.None;
		this.inventoryType = inventoryType;
        this.slotID = slotID;
		this.slotState = slotState;
    }

	public readonly InventoryType GetInventoryType()
	{
		return inventoryType;
	}

    public readonly bool IsEmpty()
    {
        return itemTypeId == ItemTypeId.None;
    }

	public readonly bool AllowsInput()
	{
		return slotState == SlotState.Input || slotState == SlotState.Both;
	}

	public readonly bool AllowsOutput()
	{
		return slotState == SlotState.Output || slotState == SlotState.Both;
	}

	public readonly int GetSlotID()
	{
		return slotID;
	}
	public readonly ItemTypeId GetItemID()
	{
		return itemTypeId;
	}

	public readonly ItemData GetItemData()
	{
		return ItemDatabase.Instance.GetItemByID(itemTypeId);
	}

	// only used when making new instance.

	public void SetSlotState(SlotState state)
	{
		slotState = state;
	}

	public void SetItem(ItemTypeId item)
	{
		itemTypeId = item;
	}

	public void RemoveItem()
	{
		itemTypeId = ItemTypeId.None;
	}
}
