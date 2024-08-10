using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSript : MonoBehaviour{

    public Rigidbody2D myRigidbody;
    public float WalkSpeed;
    public LayerMask interactableLayer; // Layer for interactable objects
    public float interactionRange = 2f; // Range to detect interactable objects

    // Start is called before the first frame update
    void Start()
    {
        
    }

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
            myRigidbody.velocity = Vector2.left * WalkSpeed;
        }

        //player go right
        if (Input.GetKey(KeyCode.D) == true)
        {
            myRigidbody.velocity = Vector2.right * WalkSpeed;
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
