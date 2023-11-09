using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public enum CameraSetting
    {
        FlyCamera,
        FollowCamera
    }

    [SerializeField] public CameraSetting setting = CameraSetting.FlyCamera;

    //Panning
    [Header("Panning")]
    public float panSpeed = 120.0f;

    //Zoom
    [Header("Zoom")]
    [Range(0.0f, 100.0f)] public float zoomSpeed = 0.01f;
    [Range(0.0f, 1.0f)] public float minZoom = 0.1f;
    [Range(0.0f, 1.0f)]public float maxZoom = 1.0f;

    //Orbit
    [Header("Orbit")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float rotationSpeed = 5.0f;
    private float targetAngle = 45.0f;
    private float currentAngle = 0.0f;

    //Follow
    [Header("Follow Camera")]
    [SerializeField] public Transform _followPoint;
    [SerializeField] public float followDistance;


    ViewportBlitter _viewport;

    private void Awake()
    {
        _viewport = GetComponentInChildren<ViewportBlitter>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (setting == CameraSetting.FlyCamera) 
        {
            Pan();
            Orbit();
            Zoom();
            return;
        }

        if (setting == CameraSetting.FollowCamera)
        {
            Orbit();
            FollowPoint();
            Zoom();
            return;
        }
        
    }

    private void Pan()
    {
        Vector2 pan = new Vector2(0.0f, 0.0f);

        if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            pan.x += 1 * panSpeed;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            pan.x -= 1 * panSpeed;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            pan.y -= panSpeed;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            pan.y += panSpeed;
        }

        _viewport.PanViewport(pan.x * Time.deltaTime, pan.y * Time.deltaTime);
    }

    private void Orbit()
    {
        float mouseX = Input.GetAxis("Mouse X");

        //Click and drag to rotate camera pivot
        if (Input.GetMouseButton(1))
        {
            targetAngle += mouseX * mouseSensitivity;
        }
        else
        {   //Let go the mouse; the camera pivot snaps to an increment of 45
            targetAngle = Mathf.Round(targetAngle / 45);
            targetAngle *= 45;
        }

        //Angle correction 0 to 360
        if (targetAngle < 0.0f) targetAngle += 360.0f;
        if (targetAngle > 360.0f) targetAngle -= 360.0f;

        currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(30, currentAngle, 0);
    }

    private void Zoom()
    {
        float scrollInput = Input.mouseScrollDelta.y;

        if (scrollInput != 0.0f)
        {
            float newZoom = _viewport.GetViewportZoom();
            newZoom += -scrollInput * zoomSpeed * Time.deltaTime;
            _viewport.ZoomViewport(Mathf.Clamp(newZoom, minZoom, maxZoom));
        }
    }

    private void FollowPoint()
    {
        gameObject.transform.position = _followPoint.position;
    }
}
