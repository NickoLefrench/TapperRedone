using UnityEngine;
using System.Linq;

namespace FMS.TapperRedone.Interactables
{
    public class CocktailFridge : InteractableObject
    {
        //array to store colour coded ingredients items as ItemType
        public Inventory.Item[] Ingredients;

        // Reference to the SpriteRenderer of the fridge
        private SpriteRenderer FridgeSpriteRenderer;

        // Assign this in the inspector to set what color category this fridge belongs to
        //editable in inspector but not public
        [SerializeField]
        private ColorCategory FridgeColorCategory;

        //short enum list for the colour window of error
        private enum ColorCategory
        {
            Red,
            Yellow,
            Blue,
            Unknown
        }

        public void Awake()
        {
            if (FridgeSpriteRenderer == null)
            {
                //getting the sprite renderer of assigned object prefab 
                FridgeSpriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public override void Interact(Characters.PlayerInteraction player)
        {
            base.Interact(player);

            // Use FridgeColorCategory directly
            Inventory.Item itemToSpawn = GetIngredientForCategory(FridgeColorCategory);

            // debugging to confirm if color exists
            if (itemToSpawn != null)
            {
                player.CurrentInventory.AddItem(itemToSpawn);
            }
            else
            {
                Debug.LogWarning("No item found for the color category: " + FridgeColorCategory);
            }


        }

        private Inventory.Item GetIngredientByType(Inventory.Item.ItemType type)
        {
            // Iterate through the Ingredients array to find the matching item
            foreach (var ingredient in Ingredients)
            {
                if (ingredient.itemType == type)
                {
                    return ingredient;
                }
            }

            // Return null if no matching item is found
            return null;
        }

        private Inventory.Item GetIngredientForCategory(ColorCategory category)
        {
            switch (category)
            {
                case ColorCategory.Red:
                    return GetIngredientByType(Inventory.Item.ItemType.RedIngredient);
                case ColorCategory.Blue:
                    return GetIngredientByType(Inventory.Item.ItemType.BlueIngredient);
                case ColorCategory.Yellow:
                    return GetIngredientByType(Inventory.Item.ItemType.YellowIngredient);
                default:
                    return null;
            }
        }
    }
}
