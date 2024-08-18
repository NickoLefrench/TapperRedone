using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public virtual void Interact(PlayerScript player)
    {
        Debug.Log($"Player {player.name} interacting with object {gameObject.name}.");
    }
}
