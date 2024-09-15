using UnityEngine;

namespace FMS.TapperRedone.Interactables
{
    public abstract class InteractableObject : MonoBehaviour
    {
        public virtual void Interact(Characters.PlayerInteraction player)
        {
            Debug.Log($"Player {player.name} interacting with object {gameObject.name}.");
        }
    }
}
