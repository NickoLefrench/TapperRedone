using FMS.TapperRedone.Characters;
using FMS.TapperRedone.Interactables;
using FMS.TapperRedone.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FMS.TapperRedone.Inventory.Item;

public class ProgressionManager : MonoBehaviour
{
    public int currentNight = 1;    //will increase which each completion

    private List<BarPatron> activePatrons = new();

    //activePatrons = PatronManager.Instance.GetActivePatrons();

    public static ProgressionManager Instance { get; private set; }

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

   //  private int currentNight = 1; necessary?

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetCurrentNight()
    {
        return currentNight;
    }

    public void SetCurrentNight(int night)
    {
        currentNight = night;
    }


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
        // Enabling beer interaction, enable/disable in BeerTap
        if (BeerTap.Instance != null)
        {
            BeerTap.Instance.SetInteractable(config.enableBeerTap);
        }
        else
        {
            Debug.LogError("BeerTap.Instance is null!");
        }

        //enabling cocktail interaction, enable/disable in CocktailMiniGame and CocktailFridge

        //the following is setup for cocktail shaker, For current PR, focusing on Beers only on Night 1
        /*CocktailShaker.Instance.SetInteractable(config.enableCocktailTools);
       
        //unsure 
        foreach (var fridge in Fridge.AllFridges)
        {
            fridge.SetInteractable(config.enableCocktailTools);
        }*/
    }

    private void UpdatePatronOrders(NightConfig config)
    {
        //Passing allowedOrders list to the PatronManager, so patrons only order from this list
        // Passing allowedOrders list to the PatronManager, so patrons only order from this list
        if (PatronManager.Instance != null)
        {
            PatronManager.Instance.SetPatronOrderPreferences(config.allowedOrders);
        }
        else
        {
            Debug.LogError("PatronManager.Instance is null!");
        }
    }

    public void SetPatronOrderPreferences(List<ItemType> orders)
    {
        activePatrons = PatronManager.Instance.GetActivePatrons();
        foreach (var patron in activePatrons)
        {
            patron.SetAllowedOrders(orders);
        }
    }

}

