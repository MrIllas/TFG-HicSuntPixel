using UnityEngine;

public class PlanarReflectionProbe
{
    public static Vector3 GetPosition(Vector3 cameraPosition, Vector3 planePosition, Vector3 planeNormal)
    {
        Vector3 proj = planeNormal * Vector3.Dot(planeNormal, cameraPosition - planePosition);
        return cameraPosition - 2 * proj;
    }

    public static Matrix4x4 GetObliqueProjection(Camera probeCamera, Vector3 planePosition, Vector3 planeNormal)
    {
        Matrix4x4 viewMatrix = probeCamera.worldToCameraMatrix;
        Vector3 viewPosition = viewMatrix.MultiplyPoint(planePosition);
        Vector3 viewNormal = viewMatrix.MultiplyVector(planeNormal);
        Vector4 plane = new Vector4(viewNormal.x, viewNormal.y, viewNormal.z, -Vector3.Dot(viewPosition, viewNormal));
        
        return probeCamera.CalculateObliqueMatrix(plane);
    }
}
