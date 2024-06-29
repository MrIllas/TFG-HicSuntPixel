using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class WaterReflectionCamera : MonoBehaviour
{
    private Camera _probe;


    private void Awake()
    {
        _probe = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        Vector3 normal = transform.forward;
        UpdateProbeTransform(Camera.main, normal);
        CalculateObliqueProjection(normal);
    }

    private void UpdateProbeTransform(Camera cam, Vector3 normal)
    {
        Vector3 proj = normal * Vector3.Dot(normal, cam.transform.position - transform.position);
        _probe.transform.position = cam.transform.position - 2 * proj;

        //transform.position = new Vector3(cam.transform.position.x, -cam.transform.position.y + transform.parent.transform.position.y, cam.transform.position.z);


        Vector3 probeForward = Vector3.Reflect(cam.transform.forward, normal);
        Vector3 probeUp = Vector3.Reflect(cam.transform.up, normal);
        _probe.transform.LookAt(_probe.transform.position + probeForward, probeUp);
    }

    private void CalculateObliqueProjection(Vector3 normal)
    {
        Matrix4x4 viewMatrix = _probe.worldToCameraMatrix;
        Vector3 viewPosition = viewMatrix.MultiplyPoint(transform.position);
        Vector3 viewNormal = viewMatrix.MultiplyVector(normal);
        Vector4 plane = new Vector4(viewNormal.x, viewNormal.y, viewNormal.z, -Vector3.Dot(viewPosition, viewNormal));
        _probe.projectionMatrix = _probe.CalculateObliqueMatrix(plane);
    }
}
