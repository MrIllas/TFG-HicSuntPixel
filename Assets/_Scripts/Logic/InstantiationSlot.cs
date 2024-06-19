using UnityEngine;

namespace Character
{
    public class InstantiationSlot : MonoBehaviour
    {
        // What slot is this?
        public ItemModelSlot slot;
        public GameObject currentModel;

        public void UnloadModel()
        {
            if (currentModel != null)
            {
                Destroy(currentModel);
            }
        }

        public void LoadModel(GameObject model)
        {
            currentModel = model;
            model.transform.parent = transform;
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
        }
    }
}