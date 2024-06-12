using UnityEngine;

namespace SaveSystem
{
    [System.Serializable]
    // Template for things that will be saved and load
    public class CharacterSaveData
    {
        [Header("SCENE INDEX")]
        public int sceneIndex = 1;

        [Header("Character Name")]
        public string characterName = "Character";

        [Header("Time Played")]
        public float secondsPlayed = 0.0f;

        [Header("World Coordinates")]
        public float xPosition = 0.0f;
        public float yPosition = 0.0f;
        public float zPosition = 0.0f;

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
    }
}