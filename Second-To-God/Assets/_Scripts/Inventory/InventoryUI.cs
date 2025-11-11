using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
    public InventoryBase inventory;
    public IStyle[] slotStyles;

    private void Start()
    {
        // Defensive: check inventory before using it
        if (inventory == null)
        {
            Debug.LogWarning("[InventoryUI]: InventoryBase is Null.");
            return;
        }

        var uiDocument = FindFirstObjectByType<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogWarning("[InventoryUI]: UIDocument not found in scene.");
            return;
        }

        // Initialize slotStyles safely
        slotStyles = new IStyle[Mathf.Max(0, inventory.InventorySize)];
        for (int i = 0; i < inventory.InventorySize; i++)
        {
            var ve = uiDocument.rootVisualElement.Q("Slot" + i);
            if (ve != null)
                slotStyles[i] = ve.style;
            else
            {
                slotStyles[i] = null;
                Debug.LogWarning($"[InventoryUI]: UI Slot element 'Slot{i}' not found.");
            }
        }

        inventory.OnInventorySlotChanged += OnSlotChanged;

        if (inventory.inventoryType == InventoryType.Player)
        {
            PlayerInventory playerInventory = inventory as PlayerInventory;
            if (playerInventory != null)
                playerInventory.OnSlotSelected += OnSlotSelected;
        }

        InitializeUISlots();
    }

    private void OnSlotChanged(InventorySlot changedSlot)
    {
        if (changedSlot.Equals(default(InventorySlot)))
        {
            Debug.LogWarning("[InventoryUI]: OnSlotChanged received a default InventorySlot.");
            return;
        }

        int slotID = changedSlot.GetSlotID();

        if (slotStyles == null || slotID < 0 || slotID >= slotStyles.Length)
        {
            Debug.LogWarning("[InventoryUI]: Slot with ID " + slotID + " not found (invalid slotStyles or index).");
            return;
        }

        var style = slotStyles[slotID];
        if (style == null)
        {
            Debug.LogWarning("[InventoryUI]: Style for slot ID " + slotID + " is null.");
            return;
        }

        // Safely get item data
        if (ItemDatabase.Instance == null)
        {
            Debug.LogWarning("[InventoryUI]: ItemDatabase.Instance is null. Clearing slot UI.");
            style.backgroundImage = new StyleBackground();
            return;
        }

        ItemData data = ItemDatabase.Instance.GetItemByID(changedSlot.GetItemID());
        if (data == null || data.Icon == null)
        {
            // No item or no icon -> clear background
            style.backgroundImage = new StyleBackground();
        }
        else
        {
            style.backgroundImage = new StyleBackground(data.Icon);
        }
    }

    private void InitializeUISlots()
    {
        if (slotStyles == null) return;

        for (int i = 0; i < slotStyles.Length; i++)
        {
            if (slotStyles[i] != null)
                slotStyles[i].backgroundImage = new StyleBackground();
        }
    }

    // Player
    private void OnSlotSelected(int previousSlotID, int slotID)
    {
        if (slotStyles == null) return;

        if (previousSlotID >= 0 && previousSlotID < slotStyles.Length && slotStyles[previousSlotID] != null)
            slotStyles[previousSlotID].scale = Vector3.one;

        if (slotID >= 0 && slotID < slotStyles.Length && slotStyles[slotID] != null)
            slotStyles[slotID].scale = Vector3.one * 1.1f;
    }
}