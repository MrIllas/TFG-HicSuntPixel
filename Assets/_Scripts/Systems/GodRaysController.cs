using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GodRaysController : MonoBehaviour
{
    public WeatherManager weatherManager;
    public Transform follow;
    [SerializeField] private Mesh layerMesh;
    [SerializeField] private Shader layerShader;
    public DayNightCycle dayNightCycle;
    public Material mat_CCC; //Material custom cloud cookie

    [SerializeField] private int shellCount = 8;
    [SerializeField] private Vector3 startingPosition = Vector3.zero;
    [SerializeField] private Vector3 gap = Vector3.zero;
    [SerializeField] private Vector3 offset = Vector3.zero;

    [Range(-0.55f, 0.55f)]public float _directionOffset = 0.0f;

    [Header("Material values")]
    [SerializeField][Range(0.0f, 1.0f)] private float eastShellOpacity = 0.2f;
    [SerializeField][Range(0.0f, 1.0f)] private float westShellOpacity = 0.2f;
    //[SerializeField][Range(0.0f, 1.0f)] private float maxOpacity = 0.2f;
    [SerializeField] private Color color = new Color(1.0f, 0.75f, 0.33f);
    [SerializeField] private float cameraDistanceFade = 9;
    [SerializeField] private float edgeFall = 2;

   // private GameObject[] shellArray;
    private Material matEastShell;
    private Material matWestShell;

    private Vector2 godRayRotRange = new Vector2(15.0f, 90.0f);
    private Vector2 cloudAlphaRange = new Vector2(40.0f, 95.0f);

    private GameObject eastShellParent;
    private GameObject westShellParent;

    private GameObject[] eastShellArray;
    private GameObject[] westShellArray;

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
                CreateMaterial(ref matEastShell, ref eastShellOpacity);
                CreateMaterial(ref matWestShell, ref westShellOpacity);
                GenerateShell();
            }
#endif
#if UNITY_STANDALONE && !UNITY_EDITOR
                CreateMaterial(ref matEastShell, ref eastShellOpacity);
                CreateMaterial(ref matWestShell, ref westShellOpacity);
                GenerateShell();
#endif
        }
    }

    private void Update()
    {
        transform.position = follow.position;

        ShellCycle();
        AlphaOverTime();

        //ControlRotation();
        color = dayNightCycle.currentLightColor;
        matEastShell.SetColor("_Color", color);
        matWestShell.SetColor("_Color", color);

        SetCloudCookie();
    }

    private void ShellCycle()
    {
        Vector3 sunAngle = dayNightCycle.transform.localEulerAngles;
        sunAngle.x = Mathf.Clamp(sunAngle.x, godRayRotRange.x, godRayRotRange.y);

        float fOffset = _directionOffset;
        if (dayNightCycle.timeOfTheDay >= 12.0f)
        {
            fOffset *= -1;
        }

        eastShellParent.transform.localRotation = Quaternion.Euler(sunAngle + (Vector3.right * fOffset));
        westShellParent.transform.localRotation = Quaternion.Euler(sunAngle + (Vector3.right * (fOffset * -1)));
    }

    private void AlphaOverTime()
    {
        if (dayNightCycle.IsItDayTime())
        {
            matEastShell.SetFloat("_Opacity", eastShellOpacity);
            matWestShell.SetFloat("_Opacity", westShellOpacity);
        }
        else
        {
            matEastShell.SetFloat("_Opacity", 0.0f);
            matWestShell.SetFloat("_Opacity", 0.0f);
        }
    }

    private void ClearShell()
    {
        if (eastShellArray == null || westShellArray == null) return;

        foreach (GameObject shell in eastShellArray)
        {
            if (shell != null)
                Destroy(shell);
        }

        foreach (GameObject shell in westShellArray)
        {
            if (shell != null)
                Destroy(shell);
        }

        if (eastShellParent != null)
            Destroy(eastShellParent);

        if (westShellParent != null)
            Destroy(westShellParent);
    }

    private void GenerateShell()
    {
        if (shellCount <= 0) return;

        eastShellParent = new GameObject("North Shell");
        westShellParent = new GameObject("South Shell");
        eastShellParent.transform.parent = transform;
        westShellParent.transform.parent = transform;
        eastShellParent.transform.localPosition = Vector3.zero;
        westShellParent.transform.localPosition = Vector3.zero;

        eastShellArray = new GameObject[shellCount];
        westShellArray = new GameObject[shellCount];

        Vector3 position = startingPosition;
        Vector3 size = new Vector3(15, 1, 15);

        Vector3 eastRotation = new Vector3(0, 0, 0);
        Vector3 westRotation = new Vector3(180f, 0, 0);

        GenerateShellArray(position, size, eastRotation, ref eastShellArray, ref eastShellParent, ref matEastShell);
        GenerateShellArray(position, size, westRotation, ref westShellArray, ref westShellParent, ref matWestShell);
    }

    private void GenerateShellArray(Vector3 position, Vector3 size, Vector3 rotation, ref GameObject[] shellArray, ref GameObject parentShell, ref Material mat)
    {
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
            layer.transform.parent = parentShell.transform;

            //Configuration of the layer
            layer.transform.localPosition = position;

            layer.transform.localScale = size;
            layer.transform.localPosition += Vector3.right * offset.x + Vector3.up * offset.y + Vector3.forward * offset.z;

            position += gap;
            layer.transform.localRotation = Quaternion.Euler(rotation);
        }
    }


    private void CreateMaterial(ref Material mat, ref float opacity)
    {
        mat = new Material(layerShader);
        mat.SetFloat("_Opacity", opacity);
        mat.SetColor("_Color", color);
        mat.SetFloat("_Camera_Distance_Fade", cameraDistanceFade);
        mat.SetFloat("_EdgeFallOff", edgeFall);
    }

    private void SetCloudCookie()
    {
        //if (dayNightCycle.IsItDayTime())
        //{
            float inputNumber = dayNightCycle.transform.localEulerAngles.x;
            float clampedInput = Mathf.Clamp(inputNumber, cloudAlphaRange.x, cloudAlphaRange.y);

            float tot = cloudAlphaRange.y - cloudAlphaRange.x;
            float v = clampedInput - cloudAlphaRange.x;

            float result = v / tot;

            mat_CCC.SetFloat("_Alpha", result);
        //}
        //else
        //{
        //    mat_CCC.SetFloat("_Alpha", 1.0f);
        //}
    }
}
