using UnityEngine;

public class GroundItem : MonoBehaviour, IInteractable
{
    public ItemData itemData;
	public bool canInteract = true;

	public void Interact(RaycastHit hit, Transform player)
	{
		if (!player.TryGetComponent(out PlayerInventory inventory))
			return;

		if (inventory.IsInventoryFull() || !canInteract)
			return;

		inventory.AddItem(itemData.itemTypeId);
		Destroy(gameObject);
	}

	public void SetCanInteract(bool value)
	{
		canInteract = value;
	}
}
