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
        public Sprite leftUIPressed;
        public Sprite leftUIUnpressed;
        public Sprite rightUIPressed;
        public Sprite rightUIUnpressed;

        public SpriteRenderer cocktailShakerRenderer; // Assign this with the Cocktail Shaker's Sprite Renderer in the Inspector
        public GameObject leftArrow; // Assign the LeftArrow object here
        public GameObject rightArrow; // Assign the RightArrow object here

        private PlayerInteraction player; // Store player reference
        private bool detectedInputDuringMiniGame = false;
        public Item itemToSpawn;

        public KeyCode leftKey = KeyCode.A;  // Key for left arrow
        public KeyCode rightKey = KeyCode.D; // Key for right arrow
        public KeyCode Quit = KeyCode.Q; //hit Q to quit, escape pauses game entirely

        public float arrowSwitchSpeed = 0.5f; // Speed at which arrows switch, adjust for difficulty
        private bool leftArrowActive = true;  // Determine which arrow is currently active
        private bool isMiniGameActive = false; // State of the minigame

        private float scoreMultiplier = 1f;  // Score multiplier based on timing
        private float perfectHitWindow = 0.2f;  // Time window for a perfect hit

        public CanvasGroup cocktailCanvasGroup; // Reference to your Canvas Group for the mini-game UI

        [Tooltip("Duration of the mini-game in seconds.")]
        public float miniGameDuration = 10f; // Default to 10 seconds, can be adjusted in the inspector

        private Coroutine rhythmCoroutine; // Coroutine to manage rhythm game


        // Start is called before the first frame update
        void Start()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;

            // Set the Cocktail Shaker to be visible at the start
            if (cocktailShakerRenderer != null)
            {
                cocktailShakerRenderer.enabled = true;
            }

            // Ensure arrows are hidden until the minigame starts
            if (leftArrow != null) leftArrow.SetActive(false);
            if (rightArrow != null) rightArrow.SetActive(false);


        }

        // Update is called once per frame
        void Update()
        {
           /* if (isMiniGameActive && Input.GetKeyDown(Quit))       LEGACY
            {
                Debug.Log("Player quit the mini-game.");
                EndCocktailMiniGame(givesDrink: false); // Ends the game without giving drink
            }*/

            if (isMiniGameActive)
                CheckForQuit();

            // Only run the mini-game logic if the game state is CocktailMiniGame
            if (GameManager.Instance.State == GameManager.GameState.CocktailMiniGame)
            {
                DetectPlayerInput();
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
            base.Interact(player);
            this.player = player; //store the player ref

            // Checks to see if player could receive a beer at end of interaction
            if (player.CurrentInventory.CanAddItem(itemToSpawn)) //must add detection to see if cx has ingredients
            {
              //  GameManager.Instance.UpdateGameState(GameManager.GameState.CocktailMiniGame); - legacy code, seeiong if other option works

                GameManager.Instance.StartCocktailMiniGame();
            }

        }

        private void StartCocktailMiniGame() //rythm mini game
        {
            isMiniGameActive = true;

            detectedInputDuringMiniGame = false;

            // Hide or show relevant elements for the minigame
            if (cocktailShakerRenderer != null)
            {
                cocktailShakerRenderer.enabled = true; // Ensure it remains visible during gameplay
            }

            // Set initial left arrow sprite
            if (leftArrow != null) 
            { 
                leftArrow.SetActive(true);
                leftArrow.GetComponent<Image>().sprite = leftUIUnpressed;
            }

            // Set initial right arrow sprite
            if (rightArrow != null)
            {
                rightArrow.SetActive(true);
                rightArrow.GetComponent<Image>().sprite = rightUIUnpressed;
            }

            // Enable the Canvas for the UI arrows
            if (cocktailCanvasGroup != null)
            {
                cocktailCanvasGroup.alpha = 1f;
                cocktailCanvasGroup.interactable = true;
                cocktailCanvasGroup.blocksRaycasts = true;
            }

            rhythmCoroutine = StartCoroutine(SwitchArrowsCoroutine());

            // Start the timer coroutine for automatic end
            StartCoroutine(MiniGameTimer());
        }

        private IEnumerator SwitchArrowsCoroutine()
        {
            while (GameManager.Instance.State == GameManager.GameState.CocktailMiniGame)
            {
                if (Input.GetKeyDown(Quit))
                {
                    Debug.Log("Quit key pressed");
                    EndCocktailMiniGame(false);
                    yield break; // Exit the coroutine
                }

                // Alternate between left and right arrow
                leftArrowActive = !leftArrowActive;

                // Switch the sprite based on active arrow
                //Sets the sprite property of the Image component on each arrow to either the pressed or unpressed state
                if (leftArrowActive)
                {
                    if (leftArrow != null && leftArrow.GetComponent<Image>() != null)
                        leftArrow.GetComponent<Image>().sprite = leftUIUnpressed;

                    if (rightArrow != null && rightArrow.GetComponent<Image>() != null)
                        rightArrow.GetComponent<Image>().sprite = rightUIPressed; 
                }
                else
                {
                    if (leftArrow != null && leftArrow.GetComponent<Image>() != null)
                        leftArrow.GetComponent<Image>().sprite = leftUIPressed;

                    if (rightArrow != null && rightArrow.GetComponent<Image>() != null)
                        rightArrow.GetComponent<Image>().sprite = rightUIUnpressed;  
                }

                // Wait for the defined switch speed
                yield return new WaitForSeconds(arrowSwitchSpeed);

                //debugging to see if left and right arrows are assigned and alternating
                Debug.Log(leftArrow);  // Check if leftArrow is null
                Debug.Log(rightArrow); // Check if rightArrow is null
                Debug.Log("LeftArrowActive: " + leftArrowActive + ", Left Arrow: " + leftArrow.GetComponent<Image>().sprite.name + ", Right Arrow: " + rightArrow.GetComponent<Image>().sprite.name);
            }
        }

        //check if player hit the btn at the right time, works in tandem with coroutine
        private void DetectPlayerInput()
        {
           //putting quit option here to see if the player can quit by hitting Q
            if (Input.GetKeyDown(Quit))
            {
                Debug.Log("Quit key pressed");
                EndCocktailMiniGame(false);
                return; // Ensure we stop further processing in this frame
            }


            //arrowSwitchSpeed contorls how quickly arrows alternate, adjust for difficulty
            //perfectHitWindow defines how much time after the switch is considered a perfect hit to gain the score multiplier

            // Detect input for the left arrow, is arrow active+did player hit btn
            //uses the difference between the current time and the time the arrow switched (time % arrowswitch), used to calculate score

            if (leftArrowActive && Input.GetKeyDown(leftKey))
            {
                float timeSinceSwitch = arrowSwitchSpeed - Mathf.Abs(Time.time % arrowSwitchSpeed);
                CalculateScore(timeSinceSwitch);
            }

            // Detect input for the right arrow
            else if (!leftArrowActive && Input.GetKeyDown(rightKey))
            {
                float timeSinceSwitch = arrowSwitchSpeed - Mathf.Abs(Time.time % arrowSwitchSpeed);
                CalculateScore(timeSinceSwitch);
            }
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
                scoreMultiplier += 0.2f;  // Lesser multiplier for a normal hit, to change
                Debug.Log("Good hit! Score multiplier: " + scoreMultiplier);
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
                    itemName = cocktailItemType.ToString(), // Set name as the type's name
                    itemType = cocktailItemType.Value // Set the item type

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

            // Revert any UI or gameplay visibility settings here, returns visibility
            if (cocktailShakerRenderer != null)
            {
                cocktailShakerRenderer.enabled = true; // Keep it visible post-gameplay as needed
            }
            if (leftArrow != null) leftArrow.SetActive(false);
            if (rightArrow != null) rightArrow.SetActive(false);

            // Disable the Canvas for the UI arrows
            if (cocktailCanvasGroup != null)
            {
                cocktailCanvasGroup.alpha = 0f;
                cocktailCanvasGroup.interactable = false;
                cocktailCanvasGroup.blocksRaycasts = false;
            }

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

            if (isMiniGameActive) // Only end if still active (not manually quit)
            {
                Debug.Log("Minigame Timer Ran out");
                EndCocktailMiniGame(givesDrink: true); // Ends game and gives drink if timer runs out
            }
        }

        //stupid quit function since update, detect player input and the switcxh arrows coroutine are all not detecting player hitting Q to quit
        private void CheckForQuit()
        {
            if (Input.GetKeyDown(Quit))
            {
                Debug.Log("Player attempted to quit the mini-game.");
                EndCocktailMiniGame(givesDrink: false); // Ends without giving drink
            }
        }
    }
}
