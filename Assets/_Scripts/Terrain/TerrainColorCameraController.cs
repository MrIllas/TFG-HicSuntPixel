using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainColorCameraController : MonoBehaviour
{
    [SerializeField]
    private Material material;

    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        //transform.position = Camera.main.transform.position;
    }


    private void LateUpdate()
    {
        material.SetVector("_TerrainColorCameraPosition", new Vector2(transform.localPosition.x, transform.localPosition.z));
        material.SetFloat("_CameraSize", _camera.orthographicSize);
    }

}
