using Items;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{
    public class PlayerInteractionManager : MonoBehaviour
    {
        PlayerManager _playerManager;

        [Header("List of interactions")]
        [SerializeField] private List<Interactable> currentInteractables; // List of interactions inside the reach of the player

        private void Awake()
        {
            _playerManager = GetComponent<PlayerManager>();
        }

        private void Start()
        {
            currentInteractables = new List<Interactable>();
        }

        private void FixedUpdate()
        {
            if (PlayerUIManager.instance.menuWindowIsOpen || PlayerUIManager.instance.popUpWindowIsOpen) return;
            CheckForInteractable();

        }

        private void CheckForInteractable()
        {
            if (currentInteractables.Count == 0) return;
            if (currentInteractables[0] == null)
            { //Prevents possible errors
                currentInteractables.RemoveAt(0);
                return;
            }
            else
            {
                PlayerUIManager.instance._playerHUDPopUpManager.SendPlayerMessagePopUp(currentInteractables[0].interactableText);
                Debug.Log("check");
            }
        }

        public void Interact()
        {
            if (currentInteractables.Count == 0) return;
            if (currentInteractables[0] != null)
            {
                currentInteractables[0].Interact(_playerManager);
                RefreshInteractionList();
            }
        }

        private void RefreshInteractionList()
        {
            for (int i = currentInteractables.Count - 1; i > - 1; --i)
            {
                if (currentInteractables[i] == null)
                {
                    currentInteractables.RemoveAt(i);
                }
            }
        }

        public void AddInteractionToList(Interactable interactable)
        {
            RefreshInteractionList();
            if(!currentInteractables.Contains(interactable)) currentInteractables.Add(interactable);
        }

        public void RemoveInteractableFromList(Interactable interactable)
        {
            if (currentInteractables.Contains(interactable)) currentInteractables.Remove(interactable);
            RefreshInteractionList();
        }
    }
}