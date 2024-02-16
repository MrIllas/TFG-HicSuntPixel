using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainColorCameraController : MonoBehaviour
{
    void Start()
    {
        TerrainData terrainData = GetComponentInParent<Terrain>().terrainData;
        Transform terrainTransform = GetComponentInParent<Transform>();

        CenterOnTerrain(terrainData, terrainTransform);
    }

    void Update()
    {
        
    }

    private void CenterOnTerrain(TerrainData terrainData, Transform terrainTransform)
    {
        Camera cam = GetComponent<Camera>();
        transform.localPosition = new Vector3(terrainData.size.x / 2, 100, terrainData.size.z / 2);
       
        cam.orthographicSize = (terrainData.size.x > terrainData.size.z ? terrainData.size.x : terrainData.size.z) / 2;
    }
}
