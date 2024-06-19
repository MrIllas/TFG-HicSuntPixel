using System.Linq;
using UnityEngine;

public class AutoLamp : MonoBehaviour
{
    private Light _light;
    private DayNightCycle _dayNight;
    private MeshRenderer _renderer;

    [Header("Materials Day/Night")]
    [SerializeField] private Material[] dayNightMaterial;

    [Header("Material Index on Mesh")]
    [SerializeField] private int lightMaterialIndexOnMesh = 1;

    private void Awake()
    {
        if (_light == null )
        {
            _light = GetComponentInChildren<Light>();
        }

        _dayNight = FindObjectOfType<DayNightCycle>();
        _renderer = GetComponent<MeshRenderer>();

        _light.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_light != null && _dayNight != null) 
        {
            if (_dayNight.IsItDayTime())
            {
                _light.enabled = false;
                if (dayNightMaterial.Length > 1)
                {
                    if (dayNightMaterial[0] != null) _renderer.materials[lightMaterialIndexOnMesh] = dayNightMaterial[0];
                    _renderer.materials = dayNightMaterial;
                }
            }
            else
            {
                _light.enabled = true;
                if (dayNightMaterial.Length > 1)
                {
                    if (dayNightMaterial[1] != null) _renderer.materials[lightMaterialIndexOnMesh] = dayNightMaterial[1];
                    _renderer.materials = dayNightMaterial;
                }
            }
            
        }   
    }
}
