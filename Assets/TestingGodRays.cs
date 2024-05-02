using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TestingGodRays : MonoBehaviour
{

    public Material mat;

    // Start is called before the first frame update
    void Start()
    {
       // mat = GetComponent<Material>();
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetVector("_Pos", transform.position);


        float d = 1 - Mathf.Cos(0.523598776f);

        //Debug.Log(d);
    }
}
