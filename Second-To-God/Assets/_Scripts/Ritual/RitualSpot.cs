using UnityEngine;

public class RitualSpot : MonoBehaviour, IInteractable
{
	public int id;
	private Ritual ritual;
	private bool triggered = false;

	private void Awake()
	{
		ritual = GetComponentInParent<Ritual>();
	}

	public void Interact(RaycastHit hit, Transform player)
	{
		if (triggered)
			return;

		PlayerInventory playerInventory = player.GetComponent<PlayerInventory>();

		if (playerInventory.CurrentEquippedItem != null && playerInventory.CurrentEquippedItem.CompareTag("Ritual Piece"))
		{
			ItemData itemData = playerInventory.GetHeldItemData();

			if (itemData == null)
				return;

			TriggerRitualItemPlaced(itemData.itemTypeId);

			// Replace with your singleplayer inventory removal API if different
			playerInventory.RemoveItem(playerInventory.SelectedSlot);
		}
	}

	public void TriggerRitualItemPlaced(ItemTypeId itemTypeId)
	{
		GameObject prefab = ItemDatabase.Instance.GetItemByID(itemTypeId).groundPrefab;
		GameObject spawnedObj = Instantiate(prefab, transform.position + Vector3.up, Quaternion.identity);

		if (spawnedObj != null)
		{
			var groundItem = spawnedObj.GetComponent<GroundItem>();
			if (groundItem != null)
				groundItem.SetCanInteract(false);
		}

		triggered = true;

		// Call the non-networked version on Ritual
		ritual.AddRitualPiece(id);
	}
}
