using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


namespace HicSuntPixel
{
    public class HSPCameraSnapper : MonoBehaviour
    {
        public Transform snapTransform;
        public HSPCameraManager _manager;
        public CameraController _cameraManager;


        //public void Snapping(bool _snap)
        //{
        //    if (_snap && !_cameraManager._rotating)
        //    {
        //        transform.position = SnapPosition(snapTransform.position);
        //    }
        //    else
        //    {
        //        transform.position = snapTransform.position;
        //    }
        //}


        //private Vector3 SnapPosition(Vector3 wp)
        //{
        //    Vector3 toReturn = Vector3.zero;

        //    float texelSize = _manager.pixelSize.y;

        //    Vector3 snapSpacePosition = transform.InverseTransformDirection(wp);
        //    Vector3 snappedSnapSpacePosition = Vector3Int.RoundToInt(snapSpacePosition / texelSize);

        //    Vector3 snapError = snappedSnapSpacePosition * texelSize;

        //    toReturn = snapError.x * transform.right + snapError.y * transform.up + snapError.z * transform.forward;

        //    return toReturn;
        //}
    }
}