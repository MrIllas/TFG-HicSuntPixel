using UnityEngine;

namespace Menus
{
    public class TitleScreenLoadMenuInputManager : MonoBehaviour
    {
        PlayerControls _playerControls;

        [Header("Title Screen Inputs")]
        [SerializeField] bool deleteCharacterSlot = false;

        private void Update()
        {
            if (deleteCharacterSlot)
            {
                deleteCharacterSlot = false;
                TitleScreenManager.instance.AttemptToDeleteCharacterSlot();
            }
        }

        private void OnEnable()
        {
            if (_playerControls == null)
            {
                _playerControls = new PlayerControls();
                _playerControls.UI.Supr.performed += i => deleteCharacterSlot = true;
            }

            _playerControls.Enable();
        }

        private void OnDisable() 
        {
            _playerControls.Disable();
        }
    }
}
