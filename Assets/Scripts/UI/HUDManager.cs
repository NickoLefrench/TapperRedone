using FMS.TapperRedone.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{

    [SerializeField] private Image drinkHUDImage;        // Drag your DrinkHUDImage here in the inspector
    [SerializeField] private Sprite defaultSprite;       // Sprite to show when no drink is held

    [SerializeField] private PlayerInventory playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure playerInventory is assigned
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory is not assigned in HUDManager!");
            return;
        }

        // Subscribe to the event
        playerInventory.OnDrinkChanged += UpdateDrinkHUD;

        // Initialize HUD with the current state
        UpdateDrinkHUD(playerInventory.HasDrink() ? playerInventory.itemsList.Find(item => item.IsDrink) : null);
    }

    private void UpdateDrinkHUD(Item drink)
    {
        if (drink !=null)
        {
            // Get the first drink in the inventory
            //Item drink = playerInventory.itemsList.Find(item => item.IsDrink);
            
            drinkHUDImage.sprite = drink.itemIcon;
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
            // Unsubscribe from the event
            playerInventory.OnDrinkChanged -= UpdateDrinkHUD; 
        }
    }
}
