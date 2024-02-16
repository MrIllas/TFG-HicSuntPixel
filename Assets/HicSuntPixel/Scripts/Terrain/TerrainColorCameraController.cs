using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainColorCameraController : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
        CenterOnMainCamera();
    }

    private void CenterOnMainCamera()
    {
        transform.position = new Vector3(Camera.main.transform.position.x, 20, Camera.main.transform.position.z);
    }
}
