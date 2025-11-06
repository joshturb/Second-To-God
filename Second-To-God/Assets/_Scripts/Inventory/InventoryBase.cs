using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class InventoryBase : MonoBehaviour
{
	public int InventorySize;
	public InventoryType inventoryType;
	public List<InventorySlot> inventorySlots;
	public event Action<InventorySlot> OnInventorySlotChanged;
	private void Awake() => inventorySlots = new();

	public virtual void Initialize()
	{
		for (int i = 0; i < InventorySize; i++)
		{
			inventorySlots.Add(new InventorySlot(inventoryType, i));
		}
	}

	public void AddItem(ItemTypeId itemTypeId)
	{
		int slotIndex = GetEmptySlotIndex();

		if (slotIndex == -1)
		{
			Debug.LogWarning("No empty Slots remaining");
			return;
		}

		InventorySlot newSlot = inventorySlots[slotIndex];
		newSlot.SetItem(itemTypeId);
		inventorySlots[slotIndex] = newSlot;
		OnInventorySlotChanged?.Invoke(newSlot);
	}

	public void AddItem(int slotID, ItemTypeId itemTypeId)
	{
		int slotIndex = GetSlotIndexByID(slotID);

		if (slotIndex == -1)
		{
			Debug.LogWarning("Slot not found");
			return;
		}
		if (!inventorySlots[slotIndex].IsEmpty())
		{
			Debug.LogWarning("Specified slot is full!");
			return;
		}

		InventorySlot newSlot = inventorySlots[slotIndex];
		newSlot.SetItem(itemTypeId);
		inventorySlots[slotIndex] = newSlot;
		OnInventorySlotChanged?.Invoke(newSlot);
	}

	public void RemoveItem(ItemTypeId itemTypeId)
	{
		int slotIndex = GetSlotIndexWithItem(itemTypeId);

		if (slotIndex == -1)
		{
			Debug.LogWarning("No slot contains that item!");
			return;
		}

		InventorySlot newSlot = inventorySlots[slotIndex];
		newSlot.RemoveItem();
		inventorySlots[slotIndex] = newSlot;
		OnInventorySlotChanged?.Invoke(newSlot);
	}

	public void RemoveItem(int slotID)
	{
		int slotIndex = GetSlotIndexByID(slotID);

		if (slotIndex == -1)
		{
			Debug.LogWarning("No slot contains that item!");
			return;
		}

		InventorySlot newSlot = inventorySlots[slotIndex];
		newSlot.RemoveItem();
		inventorySlots[slotIndex] = newSlot;
		OnInventorySlotChanged?.Invoke(newSlot);
	}

#region Helper Functions

	public int GetEmptySlotIndex()
	{
		for (int i = 0; i < inventorySlots.Count; i++)
		{
			if (inventorySlots[i].IsEmpty())
				return i;
		}
		return -1;
	}

	public int GetSlotIndexWithItem(ItemTypeId itemTypeId)
	{
		for (int i = 0; i < inventorySlots.Count; i++)
		{
			if (inventorySlots[i].GetItemID() == itemTypeId)
				return i;	
		}
		return -1;
	}

	public int GetSlotIndexByID(int slotID)
	{
		for (int i = 0; i < inventorySlots.Count; i++)
		{
			if (inventorySlots[i].GetSlotID() == slotID)
				return i;
		}
		return -1;
	}

	public bool ContainsItem(ItemTypeId itemTypeId)
	{
		for (int i = 0; i < inventorySlots.Count; i++)
		{
			if (inventorySlots[i].GetItemID() == itemTypeId)
			{
				return true;
			}
		}
		return false;
	}

#endregion
}
