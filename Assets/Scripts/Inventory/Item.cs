using UnityEngine;

namespace FMS.TapperRedone.Inventory
{
    // Can this be a struct instead?
    [System.Serializable]
    public class Item
    {
        public string itemName;
        public Sprite itemIcon; // Optionally use an icon
        public int amount;
        public ItemType itemType;

        public enum ItemType //the items that the player will be able to pick up
        {
            Beer,
            Coin,
            RedIngredient,
            BlueIngredient,
            YellowIngredient,
            GreenCocktail, // Blue and yellow
            OrangeCocktail, // Red and yellow
            PurpleCocktail, // Blue and red
        }

        public bool IsDrink
        {
            get
            {
                return itemType == ItemType.Beer
                    || itemType == ItemType.GreenCocktail
                    || itemType == ItemType.OrangeCocktail
                    || itemType == ItemType.PurpleCocktail;
            }
        }

        public bool IsIngredient
        {
            get
            {
                return itemType == ItemType.BlueIngredient
                    || itemType == ItemType.YellowIngredient
                    || itemType == ItemType.RedIngredient;
            }
        }
    }
}
