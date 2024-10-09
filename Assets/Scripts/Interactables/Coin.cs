using UnityEngine;

namespace FMS.TapperRedone.Interactables
{
    public class Coin : InteractableObject
    {
        // Score value of picking up this coin object
        public int scoreValue = 5;

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the player collided with the coin
            if (other.CompareTag("Player"))
            {
                // Update the score
                Managers.GameManager.StatManager.Score += scoreValue;

                // Destroy the coin
                Destroy(gameObject);
            }
        }
    }
}
