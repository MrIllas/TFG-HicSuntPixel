using System.Collections.Generic;
using UnityEngine;

namespace HicSuntPixel
{
    public class SeeThroughManager : MonoBehaviour
    {
        private HSPCameraManager _cameraManager;
        private CameraController _cameraController;

        [Header("Dithering")]
        [SerializeField] private float ditheringSize = 0.1f;
        [SerializeField] private float ditheringFalloff = 0.05f;

        //[Header("Raycast")]
        //[SerializeField] private float raycastRadius = 15f;

        [Header("DEBUG (Screen Position)")]
        [SerializeField] private Vector3 screenPosition;
        [SerializeField] private List<Material> currentMaterials = new List<Material>();
        [SerializeField] private List<Material> oldMaterials = new List<Material>();

        private void Awake()
        {
            _cameraManager = GetComponent<HSPCameraManager>();
            _cameraController = GetComponent<CameraController>();

        }

        private void Update()
        {
            CalculateCutoutPosition();
        }

        private void CalculateCutoutPosition()
        {
            if (_cameraController._setting != CameraController.CameraSetting.FollowCamera) return;
            List<RaycastHit> hits = new List<RaycastHit>();
            Transform target = _cameraController._target;
            LayerMask layerMasks = WorldUtilityManager.instance.GetWallsLayerMask();


            screenPosition = _cameraManager.GetWorldScreenPosition();

            Vector2 targetPositionCutout = _cameraManager._renderCamera.WorldToViewportPoint(target.position);
            targetPositionCutout.y /= (Screen.width / Screen.height);

            Vector3 offset = target.position - screenPosition;
            RaycastHit[] centerHits = Physics.RaycastAll(screenPosition, offset, offset.magnitude, layerMasks);
            hits.AddRange(centerHits);

            //Quaternion rightRotation = Quaternion.Euler(0, raycastRadius, 0);
            //Quaternion leftRotation = Quaternion.Euler(0, -raycastRadius, 0);
            //float additionalDistance = offset.magnitude + Mathf.Abs(offset.magnitude * Mathf.Sin(Mathf.Deg2Rad * raycastRadius));

            //hits.AddRange(Physics.RaycastAll(screenPosition, rightRotation * offset, additionalDistance, layerMasks));
            //hits.AddRange(Physics.RaycastAll(screenPosition, leftRotation * offset, additionalDistance, layerMasks));

            foreach (RaycastHit hit in hits)
            {
                Renderer renderer = hit.transform.GetComponent<Renderer>();
                Renderer[] renderers = hit.transform.GetComponentsInChildren<Renderer>();

                if (renderer != null)
                {
                    currentMaterials.AddRange(renderer.materials);
                }

                foreach(Renderer r in renderers)
                {
                    // If the render has the layer "WALLS"
                    if (r.gameObject.layer == 8) currentMaterials.AddRange(r.materials); 
                }

                foreach (Material mat in currentMaterials)
                {
                    mat.SetVector("_TargetPosition", targetPositionCutout);
                    mat.SetFloat("_DitheringSize", ditheringSize);
                    mat.SetFloat("_DitheringFalloff", ditheringFalloff);
                    mat.SetInt("_Dithering", 1); //Bool true

                    if (!oldMaterials.Contains(mat)) oldMaterials.Add(mat);
                }
            }

            RemoveDitheringFromOldHitMaterials();

            currentMaterials.Clear();
        }

        private void RemoveDitheringFromOldHitMaterials()
        {
            List<Material> materialsToRemove = new List<Material>();

            foreach (Material mat in oldMaterials)
            {
                if (!currentMaterials.Contains(mat))
                {
                    // Remove Dithering
                    mat.SetVector("_TargetPosition", new Vector2(-100, -100));
                    mat.SetFloat("_DitheringSize", 0);
                    mat.SetInt("_Dithering", 0); // Bool false

                    materialsToRemove.Add(mat);
                }
            }

            foreach (Material mat in materialsToRemove)
            {
                oldMaterials.Remove(mat);
            }
        }
    }
}