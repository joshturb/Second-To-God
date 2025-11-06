using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
	public InventoryBase inventory;
	public IStyle[] slotStyles;

	private void Start()
	{
		var uiDocument = FindFirstObjectByType<UIDocument>();
		slotStyles = new IStyle[inventory.InventorySize];
		for (int i = 0; i < inventory.InventorySize; i++)
		{
			slotStyles[i] = uiDocument.rootVisualElement.Q("Slot" + i).style;
		}

		if (inventory == null)
		{
			Debug.LogWarning("[InventoryUI]: InventoryBase is Null.");
			return;
		}
		inventory.OnInventorySlotChanged += OnSlotChanged;

		if (inventory.inventoryType == InventoryType.Player)
		{
			PlayerInventory playerInventory = inventory as PlayerInventory;
			playerInventory.OnSlotSelected += OnSlotSelected;
		}

		InitializeUISlots();
	}

	private void OnSlotChanged(InventorySlot changedSlot)
	{
		var slot = slotStyles[changedSlot.GetSlotID()];
		if (slot != null)
		{
			ItemData data = ItemDatabase.Instance.GetItemByID(changedSlot.GetItemID());
			slotStyles[changedSlot.GetSlotID()].backgroundImage = new StyleBackground(data.Icon);
		}
		else
		{
			Debug.LogWarning("[InventoryUI]: Slot with ID " + changedSlot.GetSlotID() + " not found.");
		}
	}

	private void InitializeUISlots()
	{
		for (int i = 0; i < inventory.InventorySize; i++)
		{
			slotStyles[i].backgroundImage = new StyleBackground();
		}
	}

	// Player
	private void OnSlotSelected(int previousSlotID, int slotID)
	{
		slotStyles[previousSlotID].scale = Vector3.one;
		slotStyles[slotID].scale = Vector3.one * 1.1f;
	}
}
