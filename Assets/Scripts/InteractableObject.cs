using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public virtual void Interact(GameObject player)
    {
        Debug.Log($"Player {player.name} interacting with object {gameObject.name}.");
    }
}
