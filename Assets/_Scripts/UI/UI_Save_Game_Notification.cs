using UnityEngine;
using System.Collections;
using TMPro;

public class UI_Save_Game_Notification : MonoBehaviour
{
    [SerializeField] private string msg = "Game saved!";
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private GameObject _popupBody;
    [SerializeField] private float displayDuration = 3.0f;
    [SerializeField] private float slideDuration = 0.5f;

    private RectTransform popupTransform;

    private Vector2 offScreenPosition;
    private Vector2 onScreenPosition;

    private void Awake()
    {
        popupTransform = GetComponent<RectTransform>();

        _popupBody.SetActive(false);
    }

    void Start()
    {
        offScreenPosition = new Vector2(popupTransform.anchoredPosition.x, popupTransform.anchoredPosition.y);
        onScreenPosition = new Vector2(popupTransform.anchoredPosition.x, popupTransform.anchoredPosition.x - 100);
    }

    public IEnumerator ShowPopup(float time)
    {
        _popupBody.SetActive(true);
        Debug.Log("Show Popup");
        label.text = msg + " (" + time.ToString("F2") + "s)";

        yield return StartCoroutine(SlideIn());
        yield return StartCoroutine(Display());
        yield return StartCoroutine(SlideOut());

        _popupBody.SetActive(false);
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

    IEnumerator Display()
    {
        float elapsedTime = 0;
        while(elapsedTime < displayDuration) 
        { 
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
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