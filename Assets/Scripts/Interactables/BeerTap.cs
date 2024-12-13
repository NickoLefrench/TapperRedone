using System.Collections;

using FMS.TapperRedone.Characters;
using FMS.TapperRedone.Data;
using FMS.TapperRedone.Inventory;
using FMS.TapperRedone.Managers;

using UnityEngine;

namespace FMS.TapperRedone.Interactables
{
    // The interactable BeerTap allows the player to play a pouring mini-game and get a beer out of it.
    public class BeerTap : InteractableObject
    {
        public GameObject MiniGameUIParent;
        public RectTransform tickRectTransform; //assgin the following in the inspector
        public RectTransform stationaryTickRectTransform;
        public float barEndPosition;
        public float timeToMove;
        public Item itemToSpawn;

        private PlayerInteraction player; // Store player reference
        private bool detectedInputDuringMiniGame = false;

        private void Start()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameManager.GameState gameState)
        {
            if (gameState != GameManager.GameState.BeerMiniGame)
            {
                return;
            }

            StartBeerMiniGame();
        }

        public override void Interact(PlayerInteraction player) //BeerMiniGame Controller
        {
            base.Interact(player);
            this.player = player; //store the player ref

            // Checks to see if player could receive a beer at end of interaction
            if (player.CurrentInventory.CanAddItem(itemToSpawn))
            {
                GameManager.Instance.UpdateGameState(GameManager.GameState.BeerMiniGame);
            }

        }
        private void Update()
        {
            if (Input.GetButtonDown("BeerPour"))//having this in update, would it be able to trigger anytime by hitting the button?
            {
                detectedInputDuringMiniGame = true;
            }
        }

        private void AwardBeer()
        {
            if (player.CurrentInventory != null && itemToSpawn != null)
            {
                // Assign the appropriate sprite for the beer (if not already assigned)
                // beerSprite should be a Sprite assigned in the inspector or initialized elsewhere
               // itemToSpawn.itemIcon = beerSprite; commented out for now, need to work on sprites

                // Add the beer to the player's inventory
                player.CurrentInventory.AddItem(itemToSpawn);
            }

            //transition back to main game
            GameManager.Instance.UpdateGameState(GameManager.GameState.MainGame);
        }

        public void StartBeerMiniGame()
        {
            MiniGameUIParent.SetActive(true);
            tickRectTransform.anchoredPosition = new Vector2(0, 0);
            StartCoroutine(AnimateMiniGame(true));
        }

        private IEnumerator AnimateMiniGame(bool movingRight) //moves the tick to the right and bounces back at the end of the bar
        {
            while (!detectedInputDuringMiniGame)
            {
                // Set up animation in current direction
                float targetX = movingRight ? barEndPosition : 0;
                bool completed = false;
                LeanTween.moveX(tickRectTransform, targetX, timeToMove).setOnComplete(() =>
                {
                    completed = true;
                });

                // Prepare next animation direction
                movingRight = !movingRight;

                // Wait until animation completes or player input comes through
                yield return new WaitUntil(() => completed || detectedInputDuringMiniGame);
            }

            if (detectedInputDuringMiniGame)
            {
                detectedInputDuringMiniGame = false;

                float distance = Mathf.Abs(tickRectTransform.anchoredPosition.x - stationaryTickRectTransform.anchoredPosition.x); //why is the hit threshold so huge??
                float hitThreshold = 2f;          //hit threshold, adjusted to smaller, was 10f
                bool perfectHit = distance <= hitThreshold;

                Debug.Log("BeerMiniGame: " + (perfectHit ? "Hit!" : "Missed."));
                int score = TunableHandler.GetTunableInt("MINI_GAME.BEER.SCORE");
                if (perfectHit)
                {
                    score += TunableHandler.GetTunableInt("MINI_GAME.BEER.PERFECT_BONUS");
                }
                GameManager.StatManager.Score += score;
                GameManager.StatManager.savedData.CurrentBeers++;
                AwardBeer();
            }

            MiniGameUIParent.SetActive(false);
        }
    }
}
