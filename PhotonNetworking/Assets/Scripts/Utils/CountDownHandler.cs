namespace Utils
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class CountDownHandler : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_TextGo;

        [SerializeField]
        private float m_ScaleSpeed = 1f;

        [SerializeField]
        private float m_FadeSpeed = 1f;

        [SerializeField]
        private int m_Count = 3;

        [SerializeField]
        private bool m_Fade = true;

        private Color startColor;

        public bool CountingDown { get; private set; }

        private void Awake()
        {
            startColor = m_TextGo.GetComponent<Text>().color;
            SetDefaultValues();
        }

        public void StartCountDown(Action onEnd, Action<int> onCounted = null)
        {
            if (m_TextGo != null)
            {
                StartCoroutine(DoCountDownRoutine(onEnd, onCounted));
            }
        }

        public void StopCountdown()
        {
            StopAllCoroutines();
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            Text textComp = m_TextGo.GetComponent<Text>();
            textComp.color = startColor;
            textComp.text = "";
            m_TextGo.transform.localScale = Vector3.zero;
        }

        private IEnumerator DoCountDownRoutine(Action onEnd, Action<int> onCounted)
        {
            CountingDown = true;

            Text goText = m_TextGo.GetComponent<Text>();

            for (int current = m_Count; current > 0; current--)
            {
                goText.text = current.ToString();
                yield return StartCoroutine(PopupTextEnumerator());
                onCounted?.Invoke(current);
                m_TextGo.transform.localScale = Vector3.zero;
                m_TextGo.GetComponent<Text>().color = startColor;
            }
            onEnd?.Invoke();

            CountingDown = false;
        }

        private IEnumerator PopupTextEnumerator()
        {
            yield return StartCoroutine(ScaleText());

            Text textComp = m_TextGo.GetComponent<Text>();
            yield return StartCoroutine(FadeText(textComp));
        }

        private IEnumerator ScaleText()
        {
            Transform textTF = m_TextGo.transform;
            float currentLerpTime = 0;

            while (textTF.localScale != Vector3.one)
            {
                currentLerpTime += Time.deltaTime * m_ScaleSpeed;
                if (currentLerpTime > 1)
                {
                    currentLerpTime = 1;
                }
                float perc = currentLerpTime / 1;
                textTF.localScale = Vector3.zero + (perc * (Vector3.one - Vector3.zero));
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator FadeText(Text text)
        {
            float currentLerpTime = 0;

            while (text.color.a != 0)
            {
                currentLerpTime += Time.deltaTime * m_FadeSpeed;
                if (currentLerpTime > 1)
                {
                    currentLerpTime = 1;
                }

                float perc = currentLerpTime / 1;
                text.color = Color.Lerp(startColor, Color.clear, perc);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}