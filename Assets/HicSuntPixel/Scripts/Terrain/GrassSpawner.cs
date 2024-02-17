using System;
using UnityEngine;

public class GrassSpawner : MonoBehaviour
{
    [Serializable]
    struct InstanceParameters
    {
        public int desiredInstances;
        public Vector3 area;
        public LayerMask hitLayer;

        public Vector2 scaleVariance;
    }

    struct InstanceData
    {
        public Matrix4x4 objectToWorld;
        public uint renderingLayerMask;
    }

    [SerializeField] private InstanceParameters _parameters;

    [SerializeField] private Camera colorTerrainCamera;

    public int realInstances;

    public Material material;
    public Mesh mesh;

    private InstanceData[] _instanceData;

    private RenderParams rp;

    private void Start()
    {
        _instanceData = new InstanceData[_parameters.desiredInstances];

        Initialize();
    }


    private void Update()
    {
        material.SetVector("_CameraPosition", new Vector2(colorTerrainCamera.transform.localPosition.x, colorTerrainCamera.transform.localPosition.z));
        material.SetFloat("_CameraSize", colorTerrainCamera.orthographicSize);

        Debug.Log(colorTerrainCamera.transform.localPosition.x + "|" + colorTerrainCamera.transform.localPosition.z);

        rp.matProps.SetVector("_CameraPosition", new Vector2(colorTerrainCamera.transform.localPosition.x, colorTerrainCamera.transform.localPosition.z));
        Graphics.RenderMeshInstanced(rp, mesh, 0, _instanceData);
    }

    private void Initialize()
    {
        TerrainData terrainData = GetComponentInParent<Terrain>().terrainData;
        InitializeGrass(terrainData);
        InitializeRenderTexture();
    }


    private void InitializeGrass(TerrainData terrainData)
    {
        rp = new RenderParams(material);

        Vector3 terrainWorldPos = transform.parent.position;

        //rp.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        rp.matProps = new MaterialPropertyBlock();
        //rp.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.Translate(new Vector3(-4.5f, 0, 0)));

        material.SetVector("_TerrainWorldPosition", terrainWorldPos);
        material.SetFloat("_CameraSize", colorTerrainCamera.orthographicSize);
        material.SetVector("_CameraPosition", new Vector2(colorTerrainCamera.transform.localPosition.x, colorTerrainCamera.transform.localPosition.z));

        rp.matProps.SetVector("_TerrainWorldPosition", terrainWorldPos);
        rp.matProps.SetFloat("_CameraSize", colorTerrainCamera.orthographicSize);
        rp.matProps.SetVector("_CameraPosition", new Vector2(colorTerrainCamera.transform.localPosition.x, colorTerrainCamera.transform.localPosition.z));

        Debug.Log(colorTerrainCamera.transform.position.x+"|"+colorTerrainCamera.transform.position.z);

        for (int i = 0; i < _parameters.desiredInstances; i++)
        {
            Vector3 rayPosition = GetRandomRayPosition();
            Ray ray = new Ray(rayPosition, Vector3.down);

            RaycastHit[] hit = Physics.RaycastAll(ray, _parameters.area.y, _parameters.hitLayer);

            if (hit.Length == 1)
            {
                Vector3 targetPos = hit[0].point;

                targetPos.y += 0.5f;

                _instanceData[i].objectToWorld = GenerateMatrix(targetPos);
                _instanceData[i].renderingLayerMask = (i & 1) == 0 ? 1u : 2u;
                realInstances++;
            }
        }
    }

    private void InitializeRenderTexture()
    {

    }

    private void OnDrawGizmos()
    {

        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireCube(transform.position, _parameters.area);
    }

    private Vector3 GetRandomRayPosition()
    {
        return new Vector3(transform.position.x +UnityEngine.Random.Range(-_parameters.area.x / 2, _parameters.area.x / 2), transform.position.y + _parameters.area.y / 2, transform.position.z + UnityEngine.Random.Range(-_parameters.area.z / 2, _parameters.area.z / 2));
    }

    private Matrix4x4 GenerateMatrix(Vector3 position)
    {
        float randomScale = UnityEngine.Random.Range(_parameters.scaleVariance.x, _parameters.scaleVariance.y);
        Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(randomScale, randomScale, randomScale));
        return matrix;
    }

}
