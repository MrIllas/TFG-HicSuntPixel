using Globals;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Character.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;

        PlayerControls _controls;

        [SerializeField] Vector2 movement;

        private void Awake()
        {
            if (instance == null) 
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            //When the scene changes, run the following logic
            SceneManager.activeSceneChanged += OnSceneChange;

            instance.enabled = false;
        }

        private void OnDestroy()
        {
            // If we destroy this object, unsubscribe the event
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // Enable player controls if we are loading into the world, otherwise disble them
            if(newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;
            }
            else
            {
                instance.enabled = false;
            }
        }

        private void OnEnable()
        {
            if( _controls == null )
            {
                _controls = new PlayerControls();

                _controls.Player.Move.performed += i => movement = i.ReadValue<Vector2>();
            }
            _controls.Enable();
        }

        
    }
}

