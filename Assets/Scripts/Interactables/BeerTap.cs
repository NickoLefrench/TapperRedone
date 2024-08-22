using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The interactable BeerTap allows the player to play a pouring mini-game and get a beer out of it.
public class BeerTap : InteractableObject
{
    public RectTransform tickRectTransform; //assgin the following in the inspector
    public RectTransform stationaryTickRectTransform;
    public float barEndPosition;
    public float timeToMove;

    public Item itemToSpawn;
    private PlayerInteraction player; // Store player reference

    public override void Interact(PlayerInteraction player) //BeerMiniGame Controller
    {
        base.Interact(player);
        this.player = player; //store the player ref
        PourMiniGame();
        
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
        tickRectTransform.anchoredPosition = new Vector2(0, 0);
        context.StartCoroutine(MoveTickCoroutine(true));
    }

    private IEnumerator MoveTickCoroutine(bool movingRight) //moves the tick to the right and bounces back at the end of the bar
    {
        while (true)
        {
            float targetX = movingRight ? barEndPosition : 0;

            bool completed = false;
            LeanTween.moveX(tickRectTransform, targetX, timeToMove).setOnComplete(() => {
                completed = true;
            });

            yield return new WaitUntil(() => completed);

            movingRight = !movingRight;

            yield return CheckForHitCoroutine();
        }
    }

    private IEnumerator CheckForHitCoroutine() //instead of having this in an update function every frame, coroutine 
    {
        while (LeanTween.isTweening(tickRectTransform)) //checks when player hits space for minigame
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                float distance = Mathf.Abs(tickRectTransform.anchoredPosition.x - stationaryTickRectTransform.anchoredPosition.x);
                float hitThreshold = 10f;

                if (distance <= hitThreshold)
                {
                    Debug.Log("Hit!");
                    AwardBeer(); //player gets the beer and bonus
                   
                    // Score Updater  
                }
                else
                {
                    Debug.Log("Missed!");

                    AwardBeer(); //player gets the beer without bonus
                }

                yield break;
            }

            yield return null;
        }
    }
}
