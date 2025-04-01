using UnityEngine;

namespace GameInteractions
{
    public abstract class InteractableObject : MonoBehaviour
    {
        public enum InteractionState { None, Near, Active }

        public abstract string GetInteractionText();
        public abstract void Interact();
        public abstract void SetInteractionState(InteractionState state);
    }
}