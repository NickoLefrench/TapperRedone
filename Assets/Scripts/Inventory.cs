using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //this class is to manage the items the player can pick up
    //player should only be able to pick up 2 items at a time
    //player can carry only 1 drink at a time

    public List<Item> items = new List<Item>();
    public GameObject itemPrefab; // Assign this in the inspector

    public int MaxStack = 1;

    public void AddItem(Item item)
    {
        items.Add(item);
        Debug.Log("Item added: " + item.itemName);
    }

    public void DropItem(Item item, Vector3 dropPosition)
    {
        // Remove the item from the inventory
        items.Remove(item);

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
