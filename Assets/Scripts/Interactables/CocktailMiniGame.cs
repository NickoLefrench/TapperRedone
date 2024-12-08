using FMS.TapperRedone.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For UI elements

using FMS.TapperRedone.Data;
using FMS.TapperRedone.Inventory;
using FMS.TapperRedone.Managers;

namespace FMS.TapperRedone.Interactables
{

    public class CocktailMiniGame : InteractableObject
    {

        // References to the sprites for the arrows
        [Header("Arrow Sprites")]
        [SerializeField] private Sprite leftUIPressed;
        [SerializeField] private Sprite leftUIUnpressed;
        [SerializeField] private Sprite rightUIPressed;
        [SerializeField] private Sprite rightUIUnpressed;

        [Header("References")]
        [SerializeField] private SpriteRenderer cocktailShakerRenderer;       // Assign this with the Cocktail Shaker's Sprite Renderer in the Inspector
        [SerializeField] private GameObject leftArrow;                        // Assign the LeftArrow object here
        [SerializeField] private GameObject rightArrow;                       // Assign the RightArrow object here
        [SerializeField] private CanvasGroup cocktailCanvasGroup;             // Reference to your Canvas Group for the mini-game UI

       
        private bool detectedInputDuringMiniGame = false;
        public Item itemToSpawn;

        [Header("Game Settings")]
        [SerializeField] private float arrowSwitchSpeed = 0.5f;              // Speed at which arrows switch, adjust for difficulty
        [SerializeField] private float perfectHitWindow = 0.2f;              // Time window for a perfect hit

        [Tooltip("Duration of the mini-game in seconds.")]
        public float miniGameDuration = 10f;                                // Default to 10 seconds, can be adjusted in the inspector

        private PlayerInteraction player;                                   // Store player reference
        private bool leftArrowActive = true;                                // Determine which arrow is currently active
        private bool isMiniGameActive = false;                              // State of the minigame
        private float scoreMultiplier = 1f;                                 // Score multiplier based on timing
        private Coroutine rhythmCoroutine;                                  // Coroutine to manage rhythm game  

        // Axis names for input (customize in Unity's Input settings)
        private const string LeftAxis = "Horizontal";
        private const string QuitKey = "Cancel";

        private void Awake()
        {
            // Null checks moved to Awake
            if (cocktailShakerRenderer == null)
                Debug.LogError("CocktailShakerRenderer is not assigned!");

            if (leftArrow == null || rightArrow == null)
                Debug.LogError("Arrow GameObjects are not assigned!");

            if (cocktailCanvasGroup == null)
                Debug.LogError("CocktailCanvasGroup is not assigned!");
        }

        // Start is called before the first frame update
        void Start()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;

            // Set the Cocktail Shaker to be visible at the start
            // Ensure initial state
            if (cocktailShakerRenderer != null)
                cocktailShakerRenderer.enabled = true;

            // Ensure arrows are hidden until the minigame starts
            if (leftArrow != null)
                leftArrow.SetActive(false);

            if (rightArrow != null)
                rightArrow.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (isMiniGameActive)
                DetectPlayerInput();

            // Quit detection is always checked
            if (Input.GetButtonDown(QuitKey))
            {
                EndCocktailMiniGame(false);
            }

           
        }

        //changes gamestate to cocktail minigame
        private void OnGameStateChanged(GameManager.GameState gameState)
        {
            // If the state changes to CocktailMiniGame, start the mini-game
            if (gameState == GameManager.GameState.CocktailMiniGame)
            {
                StartCocktailMiniGame();
            }
            else if (isMiniGameActive)
            {
                // Only end the mini-game if it was active
                EndCocktailMiniGame(false);
            }
        }

        //CocktailMiniGame Controller, similar to beer
        public override void Interact(PlayerInteraction player) 
        {
            //store the player ref
            base.Interact(player);
            this.player = player; 

            // Checks to see if player could receive a beer at end of interaction
            //must add detection to see if cx has ingredients
            if (player.CurrentInventory.CanAddItem(itemToSpawn)) 
            {

                GameManager.Instance.StartCocktailMiniGame();
            }

        }

        //rythm mini game
        private void StartCocktailMiniGame()
        {
            isMiniGameActive = true;

            // Enable UI elements
            SetUIVisibility(true);

            rhythmCoroutine = StartCoroutine(SwitchArrowsCoroutine());
            StartCoroutine(MiniGameTimer());
        }

        //alternates leftArrowActive boolean on a timed interval, ensuring regular and predictable switching
        private IEnumerator SwitchArrowsCoroutine()
        {
            while (isMiniGameActive)
            {
                leftArrowActive = !leftArrowActive;

                // Update arrow sprites
                UpdateArrowSprites();

                yield return new WaitForSeconds(arrowSwitchSpeed);
            }
        }

        //check if player hit the btn at the right time, works in tandem with coroutine
        private void DetectPlayerInput()
        {
            float input = Input.GetAxis(LeftAxis);

            // Check for input in the direction of the active arrow
            if ((leftArrowActive && input < 0) || (!leftArrowActive && input > 0))
            {
                float timeSinceSwitch = arrowSwitchSpeed - Mathf.Abs(Time.time % arrowSwitchSpeed);
                CalculateScore(timeSinceSwitch);
            }

            //arrowSwitchSpeed contorls how quickly arrows alternate, adjust for difficulty
            //perfectHitWindow defines how much time after the switch is considered a perfect hit to gain the score multiplier

            // Detect input for the left arrow, is arrow active+did player hit btn
            //uses the difference between the current time and the time the arrow switched (time % arrowswitch), used to calculate score
        }

        //multiplier increases based on timing accuracy
        private void CalculateScore(float timeSinceSwitch) 
        {

            // Increase multiplier for a perfect hit
            if (timeSinceSwitch <= perfectHitWindow)
            {
                scoreMultiplier += 0.5f; 
                Debug.Log("Perfect hit! Score multiplier: " + scoreMultiplier);
            }
            else //to modify to just receive current base score
            {
                // Lesser multiplier for a normal hit, to change
                scoreMultiplier += 0.2f; 
                Debug.Log("Good hit! Score multiplier: " + scoreMultiplier);
            }
        }

        //pressed/unpressed is assigned w/in SwitchArrowsCoroutine each time the arrow switches
        //sprites are are toggled based on the current state of leftArrowActive
        private void UpdateArrowSprites()
        {
            if (leftArrowActive)
            {
                leftArrow.GetComponent<Image>().sprite = leftUIUnpressed;
                rightArrow.GetComponent<Image>().sprite = rightUIPressed;
            }
            else
            {
                leftArrow.GetComponent<Image>().sprite = leftUIPressed;
                rightArrow.GetComponent<Image>().sprite = rightUIUnpressed;
            }
        }

        //give drink to player which corresponds to ingredients that the player has, to be adjusted
        private void GiveCorrespondingDrink() 
        {
            // Check if the player has the necessary ingredients for the cocktails
            bool hasRed = player.CurrentInventory.HasItemOfType(Inventory.Item.ItemType.RedIngredient);
            bool hasBlue = player.CurrentInventory.HasItemOfType(Inventory.Item.ItemType.BlueIngredient);
            bool hasYellow = player.CurrentInventory.HasItemOfType(Inventory.Item.ItemType.YellowIngredient);

            //to avoid uninitialized errors
            Item.ItemType? cocktailItemType = null;

            // Determine which cocktail to give based on the ingredients the player has
            if (hasRed && hasBlue)
            {
                cocktailItemType = Item.ItemType.PurpleCocktail;
                Debug.Log("Purple cocktail created");

                //remove  Red and Blue ingredients from the players inventory
                player.CurrentInventory.RemoveFirstItemOfType(Item.ItemType.RedIngredient);
                player.CurrentInventory.RemoveFirstItemOfType(Item.ItemType.BlueIngredient);
            }
            else if (hasRed && hasYellow) 
            {
                cocktailItemType = Item.ItemType.OrangeCocktail;
                Debug.Log("Orange cocktail created");

                //remove  Red and Yellow ingredients from the players inventory
                player.CurrentInventory.RemoveFirstItemOfType(Item.ItemType.RedIngredient);
                player.CurrentInventory.RemoveFirstItemOfType(Item.ItemType.YellowIngredient);
            }
            else if (hasBlue && hasYellow)
            {
                cocktailItemType = Item.ItemType.GreenCocktail;
                Debug.Log("Green cocktail created");

                //remove  Red and Blue ingredients from the players inventory
                player.CurrentInventory.RemoveFirstItemOfType(Item.ItemType.YellowIngredient);
                player.CurrentInventory.RemoveFirstItemOfType(Item.ItemType.BlueIngredient);
            }

            // If a cocktail has been determined, create the Item and add it to the inventory
            if (cocktailItemType.HasValue)
            {
                // Create a new Item for the cocktail
                Item cocktailItem = new Item
                {
                    // Set name as the type's name
                    itemName = cocktailItemType.ToString(),

                    // Set the item type
                    itemType = cocktailItemType.Value 

                    //for when we will have the specific sprite icon
                   // itemIcon = GetCocktailIcon(cocktailItemType.Value)
                };

                // Add the cocktail to the player's inventory
                player.CurrentInventory.AddItem(cocktailItem);
            }
            else
            {
                Debug.LogWarning("No valid cocktail could be made with the ingredients the player has.");
            }
        }

        // Call this when the minigame ends
        public void EndCocktailMiniGame(bool givesDrink)
        {
            // Stop all coroutines to ensure the game stops completely
            StopAllCoroutines();

            isMiniGameActive = false;

            // Hide UI elements
            SetUIVisibility(false);

            //only gives drink if the game ended naturally with the timer runing out
            if (givesDrink)
            {
                Debug.Log("Giving drink to player");
                GiveCorrespondingDrink();
            }

                //end the rythm coroutine
            if (rhythmCoroutine != null)
            {
                StopCoroutine(rhythmCoroutine);
            }

            // Return to base state via GameManager
            GameManager.Instance.EndCocktailMiniGame();
        }

        private IEnumerator MiniGameTimer()
        {
            yield return new WaitForSeconds(miniGameDuration);

            // Only end if still active (not manually quit)
            // Ends game and gives drink if timer runs out
            if (isMiniGameActive) 
            {
                Debug.Log("Minigame Timer Ran out");
                EndCocktailMiniGame(givesDrink: true); 
            }
        }

        private void SetUIVisibility(bool isVisible)
        {
            if (cocktailCanvasGroup != null)
            {
                cocktailCanvasGroup.alpha = isVisible ? 1f : 0f;
                cocktailCanvasGroup.interactable = isVisible;
                cocktailCanvasGroup.blocksRaycasts = isVisible;
            }

            if (leftArrow != null) leftArrow.SetActive(isVisible);
            if (rightArrow != null) rightArrow.SetActive(isVisible);

            if (cocktailShakerRenderer != null)
                cocktailShakerRenderer.enabled = isVisible;
        }

      
    }
}
