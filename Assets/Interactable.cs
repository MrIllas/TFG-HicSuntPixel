using Character.Player;
using UnityEngine;

namespace Items
{
    public class Interactable : MonoBehaviour
    {
        public string interactableText = "F"; // Prompt
        [HideInInspector] protected Collider trigger;
        
        protected virtual void Awake()
        {
            if (trigger == null)
                trigger = GetComponent<Collider>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerLocomotion>()._player;

            if (player != null)
            {
                player._playerInteractionManager.AddInteractionToList(this);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerLocomotion>()._player;

            if (player != null)
            {
                player._playerInteractionManager.RemoveInteractableFromList(this);
            }

            PlayerUIManager.instance._playerHUDPopUpManager.CloseAllPopUpWindows();
        }

        public virtual void Interact(PlayerManager player)
        {
            player._playerInteractionManager.RemoveInteractableFromList(this);
            PlayerUIManager.instance._playerHUDPopUpManager.CloseAllPopUpWindows();
            trigger.enabled = false;
        }
    }
}