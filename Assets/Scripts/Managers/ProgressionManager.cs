using FMS.TapperRedone.Characters;
using FMS.TapperRedone.Interactables;
using FMS.TapperRedone.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMS.TapperRedone.Inventory;

using UnityEngine;
//using static FMS.TapperRedone.Inventory.Item;

public class ProgressionManager : MonoBehaviour
{
    public int currentNight = 1;    //will increase which each completion

    private List<BarPatron> activePatrons = new();

    //activePatrons = PatronManager.Instance.GetActivePatrons();

    public static ProgressionManager Instance { get; private set; }

    // Define night-specific configurations 
    [System.Serializable]
    public class NightConfig
    {
        public bool enableBeerTap;                           //For BeerTap
        public bool enableCocktailTools;                    //for CocktailMiniGame and CocktailFridge
        public List<Item.ItemType> allowedOrders;    //for BarPatron
    }

    //defining GameState
    [SerializeField]
    private List<NightConfig> nightConfigs;

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

    public void Start()
    {
        // Ensure the current night is set to a valid value
        if (currentNight <= 0 || currentNight > nightConfigs.Count)
        {
            Debug.LogError("Invalid night configuration! Defaulting to Night 1.");
            currentNight = 1;
        }

        // Start the current night
        StartNight(currentNight);
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
        if (BeerTap.Instance != null)
        {
            BeerTap.Instance.SetInteractable(config.enableBeerTap);
        }
        else
        {
            Debug.LogError("BeerTap.Instance is null!");
        }

       
    }

    //Passing allowedOrders list to the PatronManager, so patrons only order from this list
    private void UpdatePatronOrders(NightConfig config)
    {
        if (PatronManager.Instance != null)
        {
            PatronManager.Instance.SetPatronOrderPreferences(config.allowedOrders);
        }
        else
        {
            Debug.LogError("PatronManager.Instance is null!");
        }
    }
  
}

