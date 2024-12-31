using FMS.TapperRedone.Interactables;
using FMS.TapperRedone.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FMS.TapperRedone.Inventory.Item;

public class ProgressionManager : MonoBehaviour
{
    public int currentNight = 1;    //will increase which each completion

    // Define night-specific configurations (can use ScriptableObjects instead)
    [System.Serializable]
    public class NightConfig
    {
        public bool enableBeerTap;              //For BeerTap
        public bool enableCocktailTools;        //for CocktailMiniGame and CocktailFridge
        public List<ItemType> allowedOrders;    //for BarPatron
    }

    //defining GameState
    public List<NightConfig> nightConfigs;

    // Event for other classes
    public event System.Action<int> OnNightChange;

    public void StartNight(int night)
    {
        currentNight = night;

        // Apply night-specific configuration
        NightConfig config = nightConfigs[currentNight - 1];
        UpdateInteractables(config);
        UpdatePatronOrders(config);

        // Notify listeners
        OnNightChange?.Invoke(currentNight); 

        //maybe call other classes, such as display tutorial for new mechanics?
    }

    private void UpdateInteractables(NightConfig config)
    {
        //enabling beer interaction, enable/disable in Beetap
        BeerTap.Instance.SetInteractable(config.enableBeerTap);

        //enabling cocktail interaction, enable/disable in CocktailMiniGame and CocktailFridge
        CocktailShaker.Instance.SetInteractable(config.enableCocktailTools);
       
        //unsure 
        foreach (var fridge in Fridge.AllFridges)
        {
            fridge.SetInteractable(config.enableCocktailTools);
        }
    }

    private void UpdatePatronOrders(NightConfig config)
    {
        //Passing allowedOrders list to the PatronManager, so patrons only order from this list
        PatronManager.Instance.SetPatronOrderPreferences(config.allowedOrders);
    }


}

