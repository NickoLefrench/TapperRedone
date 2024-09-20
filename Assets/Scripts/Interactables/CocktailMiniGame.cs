using FMS.TapperRedone.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using FMS.TapperRedone.Data;
using FMS.TapperRedone.Inventory;
using FMS.TapperRedone.Managers;

namespace FMS.TapperRedone.Interactables
{

    public class CocktailMiniGame : InteractableObject
    {

        private PlayerInteraction player; // Store player reference
        private bool detectedInputDuringMiniGame = false;
        public Item itemToSpawn;

        // Start is called before the first frame update
        void Start()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnGameStateChanged(GameManager.GameState gameState) //changes gamestate to cocktail minigame
        {
            if (gameState != GameManager.GameState.CocktailMiniGame)
            {
                return;
            }

            StartCocktailMiniGame();
        }

        public override void Interact(PlayerInteraction player) //CocktailMiniGame Controller, similar to beer
        {
            base.Interact(player);
            this.player = player; //store the player ref

            // Checks to see if player could receive a beer at end of interaction
            if (player.CurrentInventory.CanAddItem(itemToSpawn)) //must add detection to see if cx has ingredients
            {
                GameManager.Instance.UpdateGameState(GameManager.GameState.CocktailMiniGame);
            }

        }

        private void StartCocktailMiniGame() //rythm mini game
        {
        
        }

        private void GiveCorrespondingDrink() //give drink to player which corresponds to ingredients that the player has
        { 
        
        }
    }
}
