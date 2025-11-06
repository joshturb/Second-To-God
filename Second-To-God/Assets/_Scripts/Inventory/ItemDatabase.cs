using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
	public static ItemDatabase Instance { get; private set; }
    public List<ItemData> Items;
	
    private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		
        Items = new List<ItemData>();
        ItemData[] allItems = Resources.LoadAll<ItemData>("");
        Items.AddRange(allItems);
    }

    public ItemData GetItemByID(ItemTypeId itemID)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].itemTypeId == itemID)
                return Items[i];
        }
        return null;
    }
}