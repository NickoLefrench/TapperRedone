using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;

namespace FMS.TapperRedone.Inventory
{
    // A player's Inventory is what he currently carries; in this game, he is expected to be able to carry either one drink or up to X ingredients.
    public class PlayerInventory : MonoBehaviour
    {
        public List<Item> itemsList = new(); //creating list and initialize it
        public GameObject itemPrefab; // Assign this in the inspector

        private int MaxIngredients = 3;

        // Event to notify when a drink is added or removed
        // public delegate void DrinkChanged(); do i still need?
        public event Action<Item> OnDrinkChanged;

        public void AddItem(Item item)
        {
            if (CanAddItem(item))
            {
                itemsList.Add(item);
                Debug.Log("Item added: " + item.itemName);

                // Notify listeners about the drink change
                if (item.IsDrink)
                {
                    OnDrinkChanged?.Invoke(item);
                }
            }
            else
            {
                Debug.Log("Could not add item " + item.itemName);
            }
        }

        public bool CanAddItem(Item item)
        {
            // To complete
            // If it is a drink, make sure we have an empty invent.
            if (item.IsDrink)
            {
                return itemsList.Count == 0;
            }

            // Ingredients can be added up to max
            if (item.IsIngredient)
            {
                return itemsList.Count < MaxIngredients
                    && itemsList.All(item => item.IsIngredient);
            }

            // Other categories are not currently handled
            Assert.IsTrue(false, $"CanAddItem does not know how to handle item ${item.itemName}");
            return false;
        }

        public bool HasItemOfType(Item.ItemType itemType)
        {
            return itemsList.Exists(item => item.itemType == itemType);
        }

        public bool HasDrink()
        {
            return itemsList.Exists(item => item.IsDrink);
        }

        public Item RemoveDrink()
        {
            Item foundItem = itemsList.Find(item => item.IsDrink);
            if (foundItem == null)
            {
                return null;
            }
            else
            {
                itemsList.Remove(foundItem);

                // Notify about the drink change
                OnDrinkChanged?.Invoke(null);

                return foundItem;
            }
        }

        public Item RemoveFirstItemOfType(Item.ItemType itemType)
        {
            Item foundItem = itemsList.Find(item => item.itemType == itemType);
            if (foundItem == null)
            {
                return null;
            }
            else
            {
                itemsList.Remove(foundItem);
                return foundItem;
            }
        }
    }
}
