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

    public override void Interact(PlayerInteraction player) //BeerMiniGame Controller
    {
        base.Interact(player);
        this.player = player; //store the player ref
        PourMiniGame();
    }

	private void Update()
	{
		if (Input.GetButtonDown("BeerPour"))
        {
            detectedInputDuringMiniGame = true;
        }
	}

	void PourMiniGame()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameState.BeerMiniGame, player); //it updates the game state, keeoing the player reference
    }

    private void AwardBeer()
    {
		if (player.CurrentInventory != null && itemToSpawn != null)
		{
			player.CurrentInventory.AddItem(itemToSpawn);
		}
	}

    public void StartBeerMiniGame(MonoBehaviour context)
    {
		MiniGameUIParent.SetActive(true);
		tickRectTransform.anchoredPosition = new Vector2(0, 0);
        context.StartCoroutine(AnimateMiniGame(true));
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

			float distance = Mathf.Abs(tickRectTransform.anchoredPosition.x - stationaryTickRectTransform.anchoredPosition.x);
			float hitThreshold = 10f;

			if (distance <= hitThreshold)
			{
				Debug.Log("Hit!");
				AwardBeer(); //player gets the beer and bonus
			}
			else
			{
				Debug.Log("Missed!");
				AwardBeer(); //player gets the beer without bonus
			}
		}

        MiniGameUIParent.SetActive(false);
	}
}
