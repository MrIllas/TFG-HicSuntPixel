using UnityEngine;

public class AutoLamp : MonoBehaviour
{

    private Light _light;
    private DayNightCycle _dayNight;

    private void Awake()
    {
        _light = GetComponent<Light>();
        _dayNight = FindObjectOfType<DayNightCycle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_light != null && _dayNight != null) 
        {
            if (_dayNight.IsItDayTime())
            {
                _light.enabled = false;
            }
            else
            {
                _light.enabled = true;
            }
        }   
    }
}
