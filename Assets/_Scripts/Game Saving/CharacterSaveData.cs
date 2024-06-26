using UnityEngine;

namespace SaveSystem
{
    [System.Serializable]
    // Template for things that will be saved and load
    public class CharacterSaveData
    {
        [Header("SCENE INDEX")]
        public int sceneIndex = 3;

        [Header("Character Name")]
        public string characterName = "Character";

        [Header("Time Played")]
        public float secondsPlayed = 0.0f;

        [Header("World Coordinates")]
        public float xPosition = 55.0f;
        public float yPosition = 2.0f;
        public float zPosition = -52.5f;

        [Header("Orientation (Euler Angles)")]
        public float xRotation = 0.0f;
        public float yRotation = -45.0f;
        public float zRotation = 0.0f;

        [Header("Stats")]
        [Header("Attibutes")]
        public int vitality = 10;
        public int endurance = 25;
        [Header("Resources")]
        public int currentHealth;
        public float currentStamina;


        #region Load / Save Functions
        public void SavePosition(Vector3 p)
        {
            xPosition = p.x;
            yPosition = p.y;
            zPosition = p.z;
        }
        public void LoadPosition(ref Vector3 p)
        {
            p = new Vector3(xPosition, yPosition, zPosition);
        }

        public void SaveOrientation(Vector3 r)
        {
            xRotation = r.x;
            yRotation = r.y;
            zRotation = r.z;
        }

        public void LoadOrientation(ref Vector3 r)
        {
            r = new Vector3(xRotation, yRotation, zRotation);
        }

        #endregion
    }
}