using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerTap : InteractableObject
{

    //this class specifes that the player interacted with the beer tap and should spawn a beer in the players inventory

    public Item itemToSpawn;



    public override void Interact(GameObject player)
    {
        Inventory playerInventory = player.GetComponent<Inventory>();
        if (playerInventory != null && itemToSpawn != null)
        {
            playerInventory.AddItem(itemToSpawn);
            Destroy(gameObject); // Optionally destroy the interactable object after interaction
        }
    }

    public float interactionDistance = 3f;
    public LayerMask interactableLayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // if the player interacts with the beer tap, using raycast, 
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance, interactableLayer))
            {
                InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
                if (interactable != null)
                {
                    interactable.Interact(gameObject); // Pass the player GameObject
                }
            }
        }
    }

    void PourMiniGame()
    {

    }

}
