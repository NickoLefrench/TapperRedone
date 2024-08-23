using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

		if (player.CurrentInventory.HasItemOfType(Item.ItemType.Beer) == false) //checks to see if player interacts and also not already holding beer
		{  
			GameManager.Instance.UpdateGameState(GameManager.GameState.BeerMiniGame);
		}

	}
	private void Update()
	{
		if (Input.GetButtonDown("BeerPour") )  
        {
            detectedInputDuringMiniGame = true;
        }
	}

    private void AwardBeer()
    {
		if (player.CurrentInventory != null && itemToSpawn != null)
		{
			player.CurrentInventory.AddItem(itemToSpawn);
		}
		GameManager.Instance.UpdateGameState(GameManager.GameState.BaseMovement);
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
            LeanTween.moveX(tickRectTransform, targetX, timeToMove).setOnComplete(() => {
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
			GameManager.Instance.AddScore(score);
			AwardBeer();
		}

        MiniGameUIParent.SetActive(false);
	}
}
