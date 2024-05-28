using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referencer : MonoBehaviour
{
    public void Initialize (GameObject go)
    {
        _ref = go;
    }

    private GameObject _ref;

    public GameObject GetReference()
    {
        return _ref;
    }

    public T GetReferenceComponent<T>() where T : MonoBehaviour
    {
        return _ref.GetComponent<T>();
    }
}
