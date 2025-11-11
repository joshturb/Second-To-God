using UnityEngine;

public class GroundItem : MonoBehaviour, IInteractable, IHoverText
{
	public ItemData itemData;
	public float spinSpeed = 50f;
	public float upAndDownFrequency = 1f;
	public float upAndDownAmplitude = 0.1f;
	public bool canInteract = true;

	public string HoverText => HandleHoverText();

	private string HandleHoverText()
	{
		if (!canInteract)
			return "";

		return gameObject.CompareTag("Ritual Piece")
			? $"Pick up <color=red>{itemData.DisplayName}</color>"
			: $"Pick up {itemData.DisplayName}";
	}

	private Vector3 startPosition;

	private void Start()
	{
		startPosition = transform.position + Vector3.up * 0.1f;
	}

	private void Update()
	{
		// Use Update for smooth visuals and avoid accumulating Y offsets
		transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
		float newY = Mathf.Sin(Time.time * upAndDownFrequency) * upAndDownAmplitude;
		transform.position = startPosition + Vector3.up * newY;
	}

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
