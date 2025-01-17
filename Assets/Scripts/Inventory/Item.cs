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
        public int itemScore;

        public enum ItemType //the items that the player will be able to pick up
        {
            Beer,
            Coin,
            RedIngredient,
            BlueIngredient,
            YellowIngredient,
            PurpleCocktail, // Blue and red
            OrangeCocktail, // Red and yellow
            GreenCocktail, // Blue and yellow
        }

        public bool IsDrink
        {
            get
            {
                return itemType == ItemType.Beer
                    || itemType == ItemType.PurpleCocktail
                    || itemType == ItemType.OrangeCocktail
                    || itemType == ItemType.GreenCocktail;
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
