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

       // Ensures activation of HUD
        if (playerInventory != null)
        {
            playerInventory.OnDrinkChanged += UpdateDrinkHUD; 
        }

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
            
            //update  drink image
            drinkHUDImage.sprite = drink.itemIcon;

            // Ensure the image is visible
            drinkHUDImage.enabled = true; 
        }
        else
        {
            // Set to a default or empty sprite
            drinkHUDImage.sprite = defaultSprite;

            // Hide if no default sprite
            drinkHUDImage.enabled = defaultSprite != null;
        }
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnDrinkChanged -= UpdateDrinkHUD; // Unsubscribe from the event
        }
    }
}
