using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HSPDebug : MonoBehaviour
{
    //Text serializebles
    [SerializeField] TMP_Text textTMP;

    //Unity Screen Render Data signleton
    [SerializeField] private ViewportBlitter _viewport;

    //Data Storage
    private Vector2 screenResolution;
    private Vector2Int referenceResolution;
    private Vector2 cameraResolution;
    private float cameraRatio;

    private Vector2 renderOffsetInPixels;
    private float orthographicSize;
    private float pixelRatio;


    void Start()
    {
        
    }

    void Update()
    {
        GetData();
        UpdateText();
    }

    private void GetData()
    {
        screenResolution = new Vector2(Screen.width, Screen.height);
        referenceResolution = _viewport.referenceResolution;
        cameraResolution = _viewport.cameraResolution;
        cameraRatio = screenResolution.x / screenResolution.y;

        renderOffsetInPixels = _viewport.renderOffsetInPixels;
        orthographicSize = _viewport.orthographicSize;
        pixelRatio = _viewport.GetPixelRatio();
    }

    private void UpdateText()
    {
        textTMP.text = "Screen Resolution = " + screenResolution.x + "x" + screenResolution.y + "\n";
        textTMP.text += "Reference Resoloution = " + referenceResolution.x + "x" + referenceResolution.y + "\n";
        textTMP.text += "MC Resolution = " + cameraResolution.x + "x" + cameraResolution.y + "\n";
        textTMP.text += "MC Ratio = " + cameraRatio.ToString("F4") + "\n";
        textTMP.text += "\n";
        textTMP.text += "Render Offset In Pixels = " + renderOffsetInPixels + "\n";
        textTMP.text += "MC Orthographic Size = " + orthographicSize + "\n";
        textTMP.text += "Pixel Ratio = " + pixelRatio;
    }
}
