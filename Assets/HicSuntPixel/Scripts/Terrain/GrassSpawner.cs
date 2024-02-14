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
        public Vector2 terrainWorldPosition;
        public uint renderingLayerMask;
    }

    [SerializeField] private InstanceParameters _parameters;

    public int realInstances;

    public Material material;
    public Mesh mesh;

    private InstanceData[] _instanceData;

    private RenderParams rp;

    [SerializeField] RenderTexture terrainColor;


    private void Start()
    {
        _instanceData = new InstanceData[_parameters.desiredInstances];

        Initialize();
    }


    private void Update()
    {

        Graphics.RenderMeshInstanced(rp, mesh, 0, _instanceData);
    }

    private void Initialize()
    {
        InitializeGrass();
        InitializeRenderTexture();
    }


    private void InitializeGrass()
    {
        rp = new RenderParams(material);

        Vector3 terrainWorldPos = transform.parent.position;

        //rp.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        rp.matProps = new MaterialPropertyBlock();
        //rp.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.Translate(new Vector3(-4.5f, 0, 0)));

        rp.matProps.SetVector("_TerrainWorldPosition", terrainWorldPos);

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
                _instanceData[i].terrainWorldPosition = terrainWorldPos;
                _instanceData[i].renderingLayerMask = (i & 1) == 0 ? 1u : 2u;
                realInstances++;
            }
        }
    }

    private void InitializeRenderTexture()
    {
        //Texture2D textureArray = GetSplatMap(GetComponentInParent<Terrain>().terrainData);

        //Graphics.Blit(textureArray, terrainColor);
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

    private Texture2D GetSplatMap(TerrainData terrainData)
    {
        //https://forum.unity.com/threads/get-color-pixel-from-terrain-with-a-wheelcollider.106829/
        Texture2D terrainSplatMap;

        TerrainLayer[] terrainLayers = terrainData.terrainLayers;

        terrainSplatMap = new Texture2D(terrainColor.width, terrainColor.height);

        Color[] terrainColors = new Color[terrainColor.width * terrainColor.height];


        terrainSplatMap = terrainData.alphamapTextures[0];

        return terrainSplatMap;
    }

}
