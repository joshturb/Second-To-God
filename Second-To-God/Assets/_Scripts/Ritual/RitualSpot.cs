using UnityEngine;

public class RitualSpot : MonoBehaviour, IInteractable, IHoverText
{
	public int id;
	private Ritual ritual;
	private bool triggered = false;

	public string HoverText => HandleHoverText();

	private string HandleHoverText()
	{
		var currentItem = FindFirstObjectByType<PlayerInventory>().CurrentEquippedItem;
		return currentItem != null && currentItem.CompareTag("Ritual Piece") && !triggered
			? "<color=#FF0000><b><size=150%>Place Ritual Piece</size></b></color>"
			: "Not A Ritual Item";
	}

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

			playerInventory.HolsterItem();
			playerInventory.RemoveItem(playerInventory.SelectedSlot);
		}
	}

	public void TriggerRitualItemPlaced(ItemTypeId itemTypeId)
	{
		GameObject prefab = ItemDatabase.Instance.GetItemByID(itemTypeId).groundPrefab;
		GameObject spawnedObj = Instantiate(prefab, transform.position + new Vector3(0, 0.25f, 0), Quaternion.identity);

		spawnedObj.layer = LayerMask.NameToLayer("Default");
		if (spawnedObj != null)
		{
			if (spawnedObj.TryGetComponent<GroundItem>(out var groundItem))
				groundItem.SetCanInteract(false);
		}

		triggered = true;
		gameObject.layer = LayerMask.NameToLayer("Default");
		ritual.AddRitualPiece(id);
	}
}
