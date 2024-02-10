using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class WaterReflectionCamera : MonoBehaviour
{
    [SerializeField] private Transform waterPlane;
    [SerializeField] private Material material;
    [SerializeField] private RenderTexture renderTexture;

    private Camera _probCam;

    private void Awake()
    {
        _probCam = GetComponent<Camera>();
        _probCam.cameraType = CameraType.Reflection;
    }

    void Update()
    {
        if (waterPlane == null || material == null || renderTexture == null) return;


        SetTransform(Camera.main, GetNormal());



        _probCam.projectionMatrix = ObliqueProjection(_probCam, waterPlane.position, GetNormal());
    }


    private void SetTransform(Camera cam, Vector3 normal)
    {
        Vector3 proj = normal * Vector3.Dot(normal, cam.transform.position - waterPlane.transform.position);
        transform.position = cam.transform.position - 2 * proj;

        //Rotation
        Vector3 probeForward = Vector3.Reflect(Camera.main.transform.forward, GetNormal());
        Vector3 probeUp = Vector3.Reflect(Camera.main.transform.up, GetNormal());

        transform.LookAt(transform.position + probeForward, probeUp);
    }
    Matrix4x4 ObliqueProjection(Camera probeCamera, Vector3 planePosition, Vector3 planeNormal)
    {
        Matrix4x4 viewMatrix = probeCamera.worldToCameraMatrix;
        Vector3 viewPosition = viewMatrix.MultiplyPoint(planePosition);
        Vector3 viewNormal = viewMatrix.MultiplyVector(planeNormal);
        Vector4 plane = new Vector4(viewNormal.x, viewNormal.y, viewNormal.z, -Vector3.Dot(viewPosition, viewNormal));

        return probeCamera.CalculateObliqueMatrix(plane);
    }

    Vector3 GetNormal()
    {
        return waterPlane.transform.forward;
    }
}
