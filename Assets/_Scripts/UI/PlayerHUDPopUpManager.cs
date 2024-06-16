using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace UI
{
    public class PlayerHUDPopUpManager : MonoBehaviour
    {
        [Header("Death Pop Up")]
        [SerializeField] GameObject youAreDeadPopUpGameObject;
        [SerializeField] TextMeshProUGUI youAreDeadPopUpBackgroundText;
        [SerializeField] TextMeshProUGUI youAreDeadPopUpText;
        [SerializeField] CanvasGroup youAreDeadPopUpCanvasGroup;

        public void SendYouAreDeadPopUp()
        {
            // Activate Post Processing Effects

            youAreDeadPopUpGameObject.SetActive(true);
            youAreDeadPopUpBackgroundText.characterSpacing = 0;
            StartCoroutine(StretchPopUpTextOverTime(youAreDeadPopUpBackgroundText, 8, 8.32f));
            StartCoroutine(FadeInPopUpOverTime(youAreDeadPopUpCanvasGroup, 5));
            StartCoroutine(WaitThenFadeOutPopUpOverTime(youAreDeadPopUpCanvasGroup, 2, 5));
        }

        private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
        {
            if (duration > 0.0f)
            {
                text.characterSpacing = 0;
                float timer = 0;
                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * Time.deltaTime);
                    yield return null;
                }
            }
        }

        private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration)
        {
            if (duration > 0)
            {
                canvas.alpha = 0;
                float timer = 0;

                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);

                    yield return null;
                }
            }

            canvas.alpha = 1;
            yield return null;
        }

        private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration , float delay)
        {
            if (duration > 0)
            {
                while (delay > 0) 
                {
                    delay = delay - Time.deltaTime;
                    yield return null;
                }

                canvas.alpha = 1;
                float timer = 0;

                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                    yield return null;
                }
            }

            canvas.alpha = 0;
            yield return null;
        }
    }
}