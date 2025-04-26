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

        // Validate night number
        if (currentNight < 1)
        {
            Debug.LogWarning($"Invalid night number {currentNight} passed to StartNight(). Defaulting to Night 1.");
            currentNight = 1;
        }

        // Apply night-specific setup
        if (currentNight - 1 < nightConfigs.Count)
        {
            NightConfig config = nightConfigs[currentNight - 1];
            UpdateInteractables(config);
        }
        else
        {
            Debug.LogWarning($"No NightConfig found for night {currentNight}. Using defaults.");
            // Optionally, call some ResetToDefaultInteractables() if you want to.
        }

        // Always set up allowed patron orders based on current night
        var allowedOrders = PatronManager.Instance.GetAllowedOrders();
        PatronManager.Instance.SetPatronOrderPreferences(allowedOrders);

        // Notify listeners
        OnNightChange?.Invoke(currentNight);
    }

    private void UpdateInteractables(NightConfig config)
    {
        // Loop through all active BeerTap instances
        foreach (var beerTap in BeerTap.AllBeerTaps)
        {
            beerTap.SetInteractable(config.enableBeerTap);
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

