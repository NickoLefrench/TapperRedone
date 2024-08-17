using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A player's Inventory is what he currently carries; in this game, he is expected to be able to carry either one drink or up to X ingredients.
public class Inventory : MonoBehaviour
{
    public List<Item> itemsList = new(); //creating list and initialize it
    public GameObject itemPrefab; // Assign this in the inspector

    private int MaxIngredients = 0;

	private void Start()
	{
        MaxIngredients = TunableHandler.GetTunableInt("INVENTORY.MAX_INGREDIENTS");
	}

	public void AddItem(Item item)
    {
        if (CanAddItem(item))
        {
            itemsList.Add(item);
            Debug.Log("Item added: " + item.itemName);
        }
        else
        {
            Debug.Log("Could not add item " + item.itemName);
        }
    }

    private bool CanAddItem(Item item)
    {
        // To complete
        // If it is a drink, make sure we have an empty invent.
        if (item.IsDrink)
        {
            return itemsList.Count == 0;
        }

        // Nothing else can be added yet
        return false;
    }

    public void DropItem(Item item, Vector3 dropPosition)
    {
        // Remove the item from the inventory
        itemsList.Remove(item);

        if (itemPrefab == null) //confirm that there is an item prefab associated in the inventory script inthe scene
        {
            Debug.LogError("Item prefab is not assigned in the Inventory!");
            return;
        }


        // Instantiate the item prefab at the drop position
        GameObject droppedItem = Instantiate(itemPrefab, dropPosition, Quaternion.identity);

        //confirms whther or not the player dropped the item
        if (droppedItem != null)
        {
            Debug.Log("Dropped item: " + item.itemName + " at position: " + dropPosition);
        }
        else
        {
            Debug.LogError("Failed to drop the item.");
        }

        
        //If player give wrong drink to cx, then no points/money should be allocated
    }
}
