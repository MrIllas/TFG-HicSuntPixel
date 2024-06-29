using UnityEngine;

public class RotatingMovement : MonoBehaviour
{
    [SerializeField] float speed = 45.0f;

    [SerializeField] Transform cube;

    private void Update()
    {
        transform.RotateAround(cube.position, Vector3.up, speed * Time.deltaTime);
    }
}
