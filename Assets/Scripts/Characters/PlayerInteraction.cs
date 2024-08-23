using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Transform dropTarget; // Assign this in the inspector where the item should be dropped, i.e the npcs or drop location...maybe it should be on a layer?

	public Inventory CurrentInventory
	{
		get
		{
			return GetComponent<Inventory>();
		}
	}

    private List<InteractableObject> availableInteractables = new();
	private bool _allowedToInteract = true;

	private void Start()
	{
		GameManager.OnGameStateChanged += OnGameStateChanged;
	}

	private void OnGameStateChanged(GameManager.GameState gameState)
	{
		_allowedToInteract = gameState == GameManager.GameState.BaseMovement;
	}

	// Update is called once per frame
	void Update()
    {
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        if (_allowedToInteract && Input.GetButtonDown("Interact"))
        {
            Debug.Log(" The player tried to interact");
			bool foundInteractable = CheckForInteractable();
        }

		/*
        if (Input.GetKeyDown(KeyCode.F)) //player drops current item in invertory holding F for a cx
		{
			//debug code to ensure that an item has been assigned to the inventory
			if (CurrentInventory == null)
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

			if (CurrentInventory.itemsList.Count > 0)
            {
                // Drop the first item in the inventory as an example
                Item itemToDrop = CurrentInventory.itemsList[0];
                CurrentInventory.DropItem(itemToDrop, dropTarget.position);
            }
        }
		*/
    }

	private bool CheckForInteractable()
	{
		availableInteractables.RemoveAll(obj => obj == null);
		if (availableInteractables.Count == 0)
		{
			Debug.Log("Nothing to interact with!");
		}
		else if (availableInteractables.Count > 1)
		{
			Debug.LogWarning("Multiple interactables available in this zone, which to interact with?");
		}
		else
		{
			availableInteractables[0].Interact(this);
			return true;
		}

		return false;
    }

	public Item AttemptInventoryTransaction(Item.ItemType itemType)
	{
		if (CurrentInventory == null)
		{
			Debug.LogWarning("Inventory not assigned!");
			return null;
		}

		if (!CurrentInventory.HasItemOfType(itemType))
		{
			Debug.Log($"Could not complete item transaction for item type {itemType}.");
			return null;
		}

		Item returnedItem = CurrentInventory.RemoveFirstItemOfType(itemType);
		Debug.Log($"Inventory transaction returns item {returnedItem.itemName}");
		return returnedItem;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("Intersected " + other.gameObject.name);
		// If the object we intersected is interactable
		if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
		{
            // And as a second safety, has the InteractableObject script
            InteractableObject interactableScript = other.GetComponent<InteractableObject>();
            if (interactableScript != null)
			{
				availableInteractables.RemoveAll(obj => obj == null);
				availableInteractables.Add(interactableScript);
				Debug.Log("Intersected interactable object " + other.gameObject.name + "; available interactable objects: " + availableInteractables.Count);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		// If the object we intersected is interactable
		if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
		{
			// And as a second safety, has the InteractableObject script
			InteractableObject interactableScript = other.GetComponent<InteractableObject>();
			if (interactableScript != null)
			{
				availableInteractables.Remove(interactableScript);
				availableInteractables.RemoveAll(obj => obj == null);
				Debug.Log("Leaving interactable object " + other.gameObject.name + "; remaining interactable objects: " + availableInteractables.Count);
			}
		}
	}
}
