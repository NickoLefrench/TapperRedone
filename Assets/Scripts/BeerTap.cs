using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The interactable BeerTap allows the player to play a pouring mini-game and get a beer out of it.
public class BeerTap : InteractableObject
{
    public Item itemToSpawn;

    public override void Interact(GameObject player)
    {
        base.Interact(player);
        // PourMiniGame();
        Inventory playerInventory = player.GetComponent<Inventory>();
        if (playerInventory != null && itemToSpawn != null)
        {
            playerInventory.AddItem(itemToSpawn);
        }
    }

    void PourMiniGame()
    {

    }

}
