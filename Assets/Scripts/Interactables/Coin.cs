using UnityEngine;

namespace FMS.TapperRedone.Interactables
{
    public class Coin : InteractableObject
    {
        public int scoreValue = 5; // The value of the coin

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the player collided with the coin
            if (other.CompareTag("Player"))
            {
                // Update the score
                Managers.GameManager.Instance.AddScore(scoreValue);

                // Destroy the coin
                Destroy(gameObject);
            }
        }
    }
}
