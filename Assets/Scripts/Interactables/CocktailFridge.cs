using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CocktailFridge : InteractableObject
{
	public Item itemToSpawn;

	public override void Interact(PlayerInteraction player)
	{
		base.Interact(player);
		player.CurrentInventory.AddItem(itemToSpawn);
	}
}
