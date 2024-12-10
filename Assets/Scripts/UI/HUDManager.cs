using FMS.TapperRedone.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{

    [SerializeField] private Image drinkHUDImage;        // Drag your DrinkHUDImage here in the inspector
    [SerializeField] private Sprite defaultSprite;       // Sprite to show when no drink is held

    private PlayerInventory playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        // Assuming PlayerInventory is attached to the player
        playerInventory = FindObjectOfType<PlayerInventory>();

        // Initialize HUD
        UpdateDrinkHUD();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateDrinkHUD()
    {
        if (playerInventory.HasDrink())
        {
            // Get the first drink in the inventory
            Item drink = playerInventory.itemsList.Find(item => item.IsDrink);
            drinkHUDImage.sprite = drink.itemIcon;
            drinkHUDImage.enabled = true; // Ensure the image is visible
        }
        else
        {
            drinkHUDImage.sprite = defaultSprite; // Set to a default or empty sprite
            drinkHUDImage.enabled = defaultSprite != null; // Hide if no default sprite
        }
    }
}
