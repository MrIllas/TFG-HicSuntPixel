using UnityEngine;

public class MeshBall : MonoBehaviour
{
    static int baseColorId = Shader.PropertyToID("_BaseColor");

    [SerializeField]
    Mesh mesh = default;

    [SerializeField]
    Material material = default;

    const int size = 1023;

    Matrix4x4[] matrices = new Matrix4x4[size];
    Vector4[] baseColors = new Vector4[size];

    MaterialPropertyBlock block;

    private void Awake()
    {
        for (int i = 0; i < matrices.Length; ++i)
        {

            Quaternion q = Quaternion.Euler(Random.value * 360.0f, Random.value * 360.0f, Random.value * 360.0f);
            matrices[i] = Matrix4x4.TRS(Random.insideUnitSphere * 10.0f, q, Vector3.one * Random.Range(0.5f, 1.5f));
            baseColors[i] = new Vector4(Random.value, Random.value, Random.value, Random.Range(0.0f, 1.0f));
        }
    }

    private void Update()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
            block.SetVectorArray(baseColorId, baseColors);
        }
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, size, block);
    }
}
