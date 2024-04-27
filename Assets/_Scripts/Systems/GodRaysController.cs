using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;

public class GodRaysController : MonoBehaviour
{
    public WeatherManager weatherManager;
    public Transform follow;
    [SerializeField] private Mesh layerMesh;
    [SerializeField] private Shader layerShader;
    public DayNightCycle dayNightCycle;

    [SerializeField] private int shellCount = 8;
    [SerializeField] private Vector3 startingPosition = Vector3.zero;
    [SerializeField] private Vector3 gap = Vector3.zero;
    [SerializeField] private Vector3 offset = Vector3.zero;

    [Range(-1.0f, 1.0f)]public float _directionOffset = 0.0f;

    [Header("Material values")]
    [SerializeField][Range(0.0f, 1.0f)] private float opacity = 0.2f;
    [SerializeField] private Color color = new Color(1.0f, 0.75f, 0.33f);
    [SerializeField] private float cameraDistanceFade = 9;
    [SerializeField] private float edgeFall = 2;
    [SerializeField] [Range(-0.5f, 0.5f)] private float sunAngle = 0.0f;

    private GameObject[] shellArray;
    private Material mat;

    private PlayerControls _controls;
    private float orbitInput;
    private float targetAngle;
    [SerializeField][Range(0f, 100.0f)] float rotationSpeed = 0.5f;

    private void OnEnable()
    {
        SetInputs();
    }

    void Start()
    {
        Initiate();

        targetAngle = transform.localEulerAngles.y;
    }

    private void OnValidate()
    {
        Initiate();
    }

    private void SetInputs()
    {
        if (_controls == null)
        {
            _controls = new PlayerControls();

            _controls.Camera.Orbit.performed += i => orbitInput = i.ReadValue<float>();
        }
        _controls.Enable();
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
        transform.position = follow.position;
        
        ControlRotation();
        color = dayNightCycle.currentLightColor;
        mat.SetColor("_Color", color);
        opacity = weatherManager.cloudDensity;
        mat.SetFloat("_Opacity", opacity);
    }

    private void ControlRotation()
    {
        if (_controls.Camera.Orbit.WasPressedThisFrame())
        {
            //transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + (orbitInput * 45.0f), transform.localEulerAngles.z);
            targetAngle += (orbitInput * 45.0f);
        }

        
        
        Quaternion t = Quaternion.Euler(transform.localEulerAngles.x, targetAngle, transform.localEulerAngles.z);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, t, rotationSpeed * Time.deltaTime);
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
        Vector3 size = new Vector3(5, 1, 5);
        Vector3 rotation = new Vector3(0, -90, 0);

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

            layer.transform.localScale = size;
            layer.transform.localPosition += Vector3.right * offset.x + Vector3.up * offset.y + Vector3.forward * offset.z;
            layer.transform.localRotation = Quaternion.Euler(rotation);
        }
    }

    private void CreateMaterial()
    {
        mat = new Material(layerShader);
        mat.SetFloat("_Opacity", opacity);
        mat.SetColor("_Color", color);
        mat.SetFloat("_Camera_Distance_Fade", cameraDistanceFade);
        mat.SetFloat("_EdgeFallOff", edgeFall);
        mat.SetFloat("_SunAngle", sunAngle);
    }

    #region Generation Help Methods

    #endregion
}
