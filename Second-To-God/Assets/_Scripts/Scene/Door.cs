using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
	[SerializeField] private Transform position1, position2;

	public void Interact(RaycastHit hit, Transform player)
	{
		Vector3 distancetoPos1 = player.position - position1.position;
		Vector3 distancetoPos2 = player.position - position2.position;

		player.GetComponent<CharacterController>().enabled = false;
		player.position = distancetoPos1.sqrMagnitude < distancetoPos2.sqrMagnitude ? position2.position : position1.position;
		player.GetComponent<CharacterController>().enabled = true;
	}
}
