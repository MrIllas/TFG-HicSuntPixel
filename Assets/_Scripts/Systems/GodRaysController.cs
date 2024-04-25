using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GodRaysController : MonoBehaviour
{
    [SerializeField] private Mesh layerMesh;
    [SerializeField] private Shader layerShader;
    public DayNightCycle dayNightCycle;

    [SerializeField] private int shellCount = 8;
    [SerializeField] private Vector3 startingPosition = Vector3.zero;
    [SerializeField] private Vector3 gap = Vector3.zero;

    [Range(-1.0f, 1.0f)]public float _directionOffset = 0.0f;

    [Header("Material values")]
    [SerializeField][Range(0.0f, 1.0f)] private float opacity = 0.2f;
    [SerializeField] private Color color = new Color(1.0f, 0.75f, 0.33f);
    [SerializeField] private float cameraDistanceFade = 9;
    [SerializeField] private float edgeFall = 2;

    private GameObject[] shellArray;
    private Material mat;

    void Start()
    {
        Initiate();
    }

    private void OnValidate()
    {
        Initiate();
    }

    private void Initiate()
    {
        if(layerShader && layerMesh)
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                ClearShell();
                CreateMaterial();
                GenerateShell();
            }
#endif
#if UNITY_STANDALONE && !UNITY_EDITOR
                CreateMaterial();
                GenerateShell();
#endif
        }
    }

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(dayNightCycle.transform.localEulerAngles + (Vector3.right * _directionOffset));
        color = dayNightCycle.currentLightColor;
        mat.SetColor("_Color", color);
    }

    private void ClearShell()
    {
        if (shellArray == null) return;
        foreach (GameObject shell in shellArray)
        {
            if (shell != null)
                Destroy(shell);
        }
    }

    private void GenerateShell()
    {
        if (shellCount <= 0) return;
        shellArray = new GameObject[shellCount];

        Vector3 position = startingPosition;

        for (int i = 0; i < shellCount; i++)
        {
            // Creation
            GameObject layer = new GameObject("Layer " + i.ToString());
            MeshFilter filter = layer.AddComponent<MeshFilter>();
            MeshRenderer mr = layer.AddComponent<MeshRenderer>();
            filter.mesh = layerMesh;
            mr.material = mat;

            // Saving and setting the layer into the world
            shellArray[i] = layer;
            layer.transform.parent = transform;

            //Configuration of the layer
            layer.transform.localPosition = position;
            position += gap;

            layer.transform.localScale = new Vector3(5, 1, 5);
            layer.transform.localPosition += Vector3.down * 2.5f;
            layer.transform.localRotation = Quaternion.identity;
        }
    }

    private void CreateMaterial()
    {
        mat = new Material(layerShader);
        mat.SetFloat("_Opacity", opacity);
        mat.SetColor("_Color", color);
        mat.SetFloat("_Camera_Distance_Fade", cameraDistanceFade);
        mat.SetFloat("_EdgeFallOff", edgeFall);
    }

    #region Generation Help Methods

    #endregion
}
