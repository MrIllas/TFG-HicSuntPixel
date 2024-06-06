using HicSuntPixel;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Snapper : MonoBehaviour
{
    [SerializeField] private bool fullAssembly = false;
    
    public List<string> componentsToAdd;

    HSPCameraManager _cameraManager;
    private Transform _snapPoint;

    private GameObject _ref;

    private void Awake()
    {
        CreateSnapPoint();
    }

    private void Start()
    {
        if (!_cameraManager)
        {
            _cameraManager = FindObjectOfType<HSPCameraManager>();
        }
    }

    private void LateUpdate()
    {
        transform.position = SnapPosition(_snapPoint.position);
    }

    private Vector3 SnapPosition(Vector3 wp)
    {
        float pixelSize = 2.0f * _cameraManager._renderCamera.orthographicSize / _cameraManager.realResolution.y;
        Transform RCTransform = _cameraManager._renderCamera.transform;

        Vector3 aux = RCTransform.InverseTransformDirection(wp);
        aux = Vector3Int.RoundToInt(aux / pixelSize);

        Vector3 snappedVec = aux * pixelSize;

        Vector3 toReturn;
        toReturn = snappedVec.x * RCTransform.right;
        toReturn += snappedVec.y * RCTransform.up;
        toReturn += snappedVec.z * RCTransform.forward;

        return toReturn;
    }

    private void CreateSnapPoint()
    {
        _ref = new GameObject(transform.name + " (Snap Point)");

        //Assign it to the same parent of this object
        _ref.transform.parent = transform.parent;

        _ref.transform.localPosition = transform.localPosition;
        _ref.transform.localRotation = transform.localRotation;
        _ref.transform.localScale = transform.localScale;

        if (componentsToAdd.Count > 0)
        {
            for (int i = 0; i < componentsToAdd.Count; i++)
            {
                if (componentsToAdd[i] != null)
                {
                    Type scriptType = null;
                    if (fullAssembly)
                        scriptType = Type.GetType(componentsToAdd[i] + ", FullAssemblyName");
                    else
                        scriptType = Type.GetType(componentsToAdd[i]);

                    if (scriptType != null)
                    {
                        Component myComponent = gameObject.GetComponent(scriptType);
                        Component comp = _ref.AddComponent(scriptType);
                        
                        //Debug.Log(comp);
                        CopyValues(myComponent, comp);

                        Destroy(myComponent);
                    }
                }
            }
            // Add reference component and initialize it
            _ref.AddComponent<Referencer>().Initialize(this.gameObject);

            // Add Character Controller (if there is one)
            CharacterController oCC = gameObject.GetComponent<CharacterController>();
            if (oCC)
            {
                _ref.AddComponent<CharacterController>();
                Destroy(oCC);
            }

        }

        _snapPoint = _ref.transform;
    }

    void CopyValues(Component from, Component to)
    {
        var json = JsonUtility.ToJson(from);
        JsonUtility.FromJsonOverwrite(json, to);
    }

    public GameObject GetReference()
    {
        return _ref;
    }

    public T GetReferenceComponent<T>() where T : MonoBehaviour
    {
        return _ref.GetComponent<T>();
    }
}