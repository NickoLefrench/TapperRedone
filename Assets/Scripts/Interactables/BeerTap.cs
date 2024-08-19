using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The interactable BeerTap allows the player to play a pouring mini-game and get a beer out of it.
public class BeerTap : InteractableObject
{
    public Item itemToSpawn;

    public override void Interact(PlayerInteraction player)
    {
        base.Interact(player);
        PourMiniGame();
        // AwardBeer(player);
    }

    void PourMiniGame()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameState.BeerMiniGame);
    }

    private void AwardBeer(PlayerInteraction player)
    {
		if (player.CurrentInventory != null && itemToSpawn != null)
		{
			player.CurrentInventory.AddItem(itemToSpawn);
		}
	}
}
