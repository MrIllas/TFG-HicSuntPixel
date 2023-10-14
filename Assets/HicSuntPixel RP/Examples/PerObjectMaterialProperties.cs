using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
    static MaterialPropertyBlock block;

    static int baseColorId = Shader.PropertyToID("_BaseColor");
    static int cutoffId = Shader.PropertyToID("_Cutoff");
    static int metallicId = Shader.PropertyToID("_Metallic");
    static int smoothnessId = Shader.PropertyToID("_Smoothness");

    [SerializeField]
    Color baseColor = Color.white;

    [SerializeField, Range(0f, 1f)] float cutoff = 0.5f;
    [SerializeField, Range(0f, 1f)] float metallic = 0.0f;
    [SerializeField, Range(0f, 1f)] float smoothness = 0.0f;


    private void OnValidate()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }
        block.SetColor(baseColorId, baseColor);
        block.SetFloat(cutoffId, cutoff);
        block.SetFloat(metallicId, metallic);
        block.SetFloat(smoothnessId, smoothness);
        GetComponent<Renderer>().SetPropertyBlock(block);
    }

    private void Awake()
    {
        OnValidate();
    }
}
