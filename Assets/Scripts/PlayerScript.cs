using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float WalkSpeed;
    public LayerMask interactableLayer; // Layer for interactable objects
    public float interactionRange = 2f; // Range to detect interactable objects

    public Inventory playerInventory = new ();
    public Transform dropTarget; // Assign this in the inspector where the item should be dropped, i.e the npcs or drop location...maybe it should be on a layer?

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleInteraction();

    }

    void HandleMovement()
    {
        //player go left
        if (Input.GetKey(KeyCode.A) == true)
        {
            transform.Translate(Vector2.left * WalkSpeed);
        }

        //player go right
        if (Input.GetKey(KeyCode.D) == true)
		{
			transform.Translate(Vector2.right * WalkSpeed);
		}

        //player interact
        if (Input.GetKey(KeyCode.E) == true)
        {
            Debug.Log(" The player tried to interact");
        }
    }

    private void HandleInteraction()
    {
        if (Input.GetKey(KeyCode.E) == true)
        {
            Debug.Log(" The player tried to interact");
            CheckForInteractable();
        }

        //debug code to ensure that an item has been assigned to the inventory
        if (playerInventory == null)
        {
            Debug.LogError("Player inventory is not assigned!");
            return;
        }

        //debug code to ensure that that confirms that there is a drop point
        //drop point should/will eventually be npcs who walk up to the bar and have ordered their drink
        if (dropTarget == null)
        {
            Debug.LogError("Drop target is not assigned!");
            return;
        }

        if (Input.GetKeyDown(KeyCode.F)) //player drops current item in invertory holding F for a cx
        {
            if (playerInventory.itemsList.Count > 0)
            {
                // Drop the first item in the inventory as an example
                Item itemToDrop = playerInventory.itemsList[0];
                playerInventory.DropItem(itemToDrop, dropTarget.position);
            }
        }
    }

    private void CheckForInteractable()
    {
        Collider2D[] interactables = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);

        foreach (var interactable in interactables)
        {
            InteractableObject interactableComponent = interactable.GetComponent<InteractableObject>();
            if (interactableComponent != null)
            {
                Debug.Log("Interacting with: " + interactable.gameObject.name);
                // Optionally call an interaction method on the interactable
                // interactableComponent.Interact();
            }
        }
    }
}
