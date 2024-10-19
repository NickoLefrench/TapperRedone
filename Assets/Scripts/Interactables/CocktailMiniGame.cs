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

        private SpriteRenderer spriteRenderer; // Ref to cocktail SpriteRenderer

        private PlayerInteraction player; // Store player reference
        private bool detectedInputDuringMiniGame = false;
        public Item itemToSpawn;

        public KeyCode leftKey = KeyCode.A;  // Key for left arrow
        public KeyCode rightKey = KeyCode.D; // Key for right arrow

        public float arrowSwitchSpeed = 0.5f; // Speed at which arrows switch, adjust for difficulty
        private bool leftArrowActive = true;  // Determine which arrow is currently active
        private bool isMiniGameActive = false; // State of the minigame

        private float scoreMultiplier = 1f;  // Score multiplier based on timing
        private float perfectHitWindow = 0.2f;  // Time window for a perfect hit

        private Coroutine rhythmCoroutine; // Coroutine to manage rhythm game

        // Start is called before the first frame update
        void Start()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;

            // Set initial sprites
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Initialize the sprite to start with left arrow unpressed
            spriteRenderer.sprite = leftUIUnpressed;

        }

        // Update is called once per frame
        void Update()
        {
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
            // Otherwise, end the mini-game
            else
            {
                EndCocktailMiniGame();
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
                GameManager.Instance.UpdateGameState(GameManager.GameState.CocktailMiniGame);
            }

        }

        private void StartCocktailMiniGame() //rythm mini game
        {
            detectedInputDuringMiniGame = false;

            // Start the rhythm switching coroutine
            rhythmCoroutine = StartCoroutine(SwitchArrowsCoroutine());
        }

        private IEnumerator SwitchArrowsCoroutine()
        {
            while (GameManager.Instance.State == GameManager.GameState.CocktailMiniGame)
            {
                // Alternate between left and right arrow
                leftArrowActive = !leftArrowActive;

                // Switch the sprite based on active arrow
                if (leftArrowActive)
                {
                    spriteRenderer.sprite = leftUIUnpressed;  // Left arrow active (unpressed)
                }
                else
                {
                    spriteRenderer.sprite = rightUIPressed;   // Right arrow active (pressed)
                }

                // Wait for the defined switch speed
                yield return new WaitForSeconds(arrowSwitchSpeed);
            }
        }

        //check if player hit the btn at the right time, works in tandem with coroutine
        private void DetectPlayerInput()
        {
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

            // Determine which cocktail to give based on the ingredients the player has
            Inventory.Item cocktailItem = null;

            if (hasRed && hasBlue)
            {
                cocktailItem = GetCocktailByType(Inventory.Item.ItemType.PurpleCocktail);
                Debug.Log("Purple cocktail created");
            }
            else if (hasRed && hasYellow) 
            {
                cocktailItem = GetCocktailByType(Inventory.Item.ItemType.OrangeCocktail);
                Debug.Log("Orange cocktail created");
            }
            else if (hasBlue && hasYellow)
            {
                cocktailItem = GetCocktailByType(Inventory.Item.ItemType.GreenCocktail);
                Debug.Log("Green cocktail created");
            }
        }

        // Call this when the minigame ends
        public void EndCocktailMiniGame()
        {
            isMiniGameActive = false;

            if (rhythmCoroutine != null)
            {
                StopCoroutine(rhythmCoroutine);
            }

            GiveCorrespondingDrink();
        }
    }
}
