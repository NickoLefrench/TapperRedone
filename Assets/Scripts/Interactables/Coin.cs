using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : InteractableObject
{
    public int scoreValue = 5; // The value of the coin
    private Score scoreScript; // Reference to the Score script

    // Start is called before the first frame update
    void Start()
    {
        // Find the Score script attached to the Canvas
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            scoreScript = canvas.GetComponent<Score>();
        }

        if (scoreScript == null)
        {
            Debug.LogError("Score script not found on the Canvas!");
        }
    }

	private void OnTriggerEnter2D(Collider2D other)
	{
        // Check if the player collided with the coin
        if (other.CompareTag("Player"))
        {
            // Update the score
            if (scoreScript != null)
            {
                scoreScript.UpdateScore(scoreScript.score + scoreValue);
            }

            // Destroy the coin
            Destroy(gameObject);
        }
    }
}
