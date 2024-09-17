using UnityEngine;
using System.Linq;

namespace FMS.TapperRedone.Interactables
{
    public class CocktailFridge : InteractableObject
    {
        public Inventory.Item[] Ingredients; //array to store colour coded ingredients items as ItemType
        private SpriteRenderer FridgeSpriteRenderer; // Reference to the SpriteRenderer of the fridge

        private enum ColorCategory //short enum list for the colour window of error
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
           

            if (FridgeSpriteRenderer != null)
            {
                Color fridgeColor = FridgeSpriteRenderer.color;
                Inventory.Item itemToSpawn = GetItemForColor(fridgeColor);

                if (itemToSpawn != null)  //debugging to confirm if colour exists
                {
                    player.CurrentInventory.AddItem(itemToSpawn);
                }
                else
                {
                    Debug.LogWarning("No item found for the color: " + fridgeColor);
                }
            }
            else
            {
                Debug.LogError("SpriteRenderer component is missing!");
            }
        }

        private Inventory.Item GetItemForColor(Color color) //which ingredient to give to player based on the fridge colour
        {
            switch (GetColorCategory(color))
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

        private ColorCategory GetColorCategory(Color color) //maps the Color to a ColorCategory enum predifined
        {
            // Here we compare colors with a small tolerance for floating-point precision issues
            if (ColorsAreClose(color, new Color(0.934f, 0f, 0f, 1f)))
                return ColorCategory.Red;

            if (ColorsAreClose(color, new Color(1f, 0.9f, 0f, 1f)))
                return ColorCategory.Yellow;

            if (ColorsAreClose(color, new Color(0f, 0.7f, 1f, 1f)))
                return ColorCategory.Blue;

            return ColorCategory.Unknown;
        }

        private bool ColorsAreClose(Color color1, Color color2, float tolerance = 0.1f) //some math from chatgpt to create a window of error of the adjusted colors on the prefab
        {                                                                               //compares colors with a tolerance to handle small differences due to floating-point precision
            return Mathf.Abs(color1.r - color2.r) < tolerance &&                        //tolerance can be adjusted
                   Mathf.Abs(color1.g - color2.g) < tolerance &&                        //this hurt my brain  a bit
                   Mathf.Abs(color1.b - color2.b) < tolerance &&
                   Mathf.Abs(color1.a - color2.a) < tolerance;
        }

        private Inventory.Item GetIngredientByType(Inventory.Item.ItemType type) //retrives the Item (itemClass) from Inventory Class based on the Ingredients array
        {
            return Ingredients.FirstOrDefault(item => item.itemType == type);
        }
    }
    
}


