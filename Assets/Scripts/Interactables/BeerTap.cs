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
        public RectTransform tickRectTransform;                   //assgin the following in the inspector
        public RectTransform stationaryTickRectTransform;
        public float barEndPosition;
        public float timeToMove;
        public Item itemToSpawn;
        [SerializeField] private Sprite beerSprite;               // Sprite for the beer

        private PlayerInteraction player;                         // Store player reference
        private bool detectedInputDuringMiniGame = false;

        //setting up BeerTap as singleton
        public static BeerTap Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

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

        //BeerMiniGame Controller
        public override void Interact(PlayerInteraction player)
        {
            base.Interact(player);

            //store the player ref
            this.player = player; 

            // Checks to see if player could receive a beer at end of interaction
            if (player.CurrentInventory.CanAddItem(itemToSpawn))
            {
                GameManager.Instance.UpdateGameState(GameManager.GameState.BeerMiniGame);
            }

        }
        private void Update()
        {
            if (Input.GetButtonDown("BeerPour"))
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
                itemToSpawn.itemIcon = beerSprite; 

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

        //moves the tick to the right and bounces back at the end of the bar
        private IEnumerator AnimateMiniGame(bool movingRight) 
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

                //why is the hit threshold so huge??
                float distance = Mathf.Abs(tickRectTransform.anchoredPosition.x - stationaryTickRectTransform.anchoredPosition.x);

                //hit threshold, adjusted to smaller, was 10f
                float hitThreshold = 2f;          
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

        public void SetInteractable(bool isInteractable)
        {
            // enable/disable the BeerTap's interaction with each night, called in progressionManager
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = isInteractable;
            }
            Debug.Log($"BeerTap interactable set to {isInteractable}");
        }
    }
}
