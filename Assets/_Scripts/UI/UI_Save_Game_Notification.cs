using UnityEngine;
using System.Collections;

public class UI_Save_Game_Notification : MonoBehaviour
{
    private  RectTransform popupTransform; 
    private float slideDuration = 0.5f;
    private float displayDuration = 3.0f;
    private Vector2 offScreenPosition;
    private Vector2 onScreenPosition;

    private void Awake()
    {
        popupTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        offScreenPosition = new Vector2(popupTransform.anchoredPosition.x, popupTransform.anchoredPosition.y);
        onScreenPosition = new Vector2(popupTransform.anchoredPosition.x, popupTransform.anchoredPosition.x - 100);
    }

    public IEnumerator ShowPopup()
    {
        Debug.Log("Show Popup");

        yield return StartCoroutine(SlideIn());
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(SlideOut());

        gameObject.SetActive(false);
    }

    IEnumerator SlideIn()
    {
        float elapsedTime = 0;
        while (elapsedTime < slideDuration)
        {
            popupTransform.anchoredPosition = Vector2.Lerp(offScreenPosition, onScreenPosition, elapsedTime / slideDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        popupTransform.anchoredPosition = onScreenPosition;
    }

    IEnumerator SlideOut()
    {
        float elapsedTime = 0;
        while (elapsedTime < slideDuration)
        {
            popupTransform.anchoredPosition = Vector2.Lerp(onScreenPosition, offScreenPosition, elapsedTime / slideDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        popupTransform.anchoredPosition = offScreenPosition;
    }
}