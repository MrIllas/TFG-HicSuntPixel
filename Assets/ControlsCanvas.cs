using UnityEngine;

public class ControlsCanvas : MonoBehaviour
{
    [SerializeField] private GameObject main; 


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            main.SetActive(!main.activeInHierarchy);
        }
    }
}
