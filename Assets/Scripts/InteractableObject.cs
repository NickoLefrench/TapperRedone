using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string interactionMessage = "The Player has interacted with the object";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player is in the trigger zone
        if (other.CompareTag("Player"))
        {
            // Notify the player that interaction is possible
            Debug.Log(interactionMessage);
            // You could also use UI elements to show interaction messages
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Clear the interaction message or hide UI elements
            Debug.Log("Player left the interaction zone");
        }
    }
}
