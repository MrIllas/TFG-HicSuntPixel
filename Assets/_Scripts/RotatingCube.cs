using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingCube : MonoBehaviour
{

    [SerializeField] float speed = 45.0f;

    [SerializeField] Transform cube;

    private void Start()
    {
        //cube = GetComponentInChildren<Transform>();
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 1, 0), Mathf.Deg2Rad * speed * Time.deltaTime);
        //cube.rotation = 
    }
}
