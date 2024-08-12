using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Item.cs
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
    }
}

