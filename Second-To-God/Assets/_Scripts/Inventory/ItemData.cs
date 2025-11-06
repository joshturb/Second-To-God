using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject, IEquatable<ItemData>
{
    public string DisplayName;
    public Sprite Icon;
    public ItemTypeId itemTypeId;
	public Vector3 holdRotation;
	public Vector3 groundRotation;
    public GameObject holdingPrefab;
    public GameObject groundPrefab;

	private void Awake()
	{
		DisplayName ??= itemTypeId.ToString();	
	}

    public bool Equals(ItemData other)
    {
        return other.itemTypeId == itemTypeId;
    }
}
