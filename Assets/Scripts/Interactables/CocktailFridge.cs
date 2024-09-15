namespace FMS.TapperRedone.Interactables
{
	public class CocktailFridge : InteractableObject
	{
		public Inventory.Item itemToSpawn;

		public override void Interact(Characters.PlayerInteraction player)
		{
			base.Interact(player);
			player.CurrentInventory.AddItem(itemToSpawn);
		}
	}
}
