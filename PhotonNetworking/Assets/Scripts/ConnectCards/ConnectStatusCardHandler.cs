namespace ConnectCards
{
    using Photon.Pun;
    using Photon.Realtime;
    using System;
    using System.Collections;
    using Utils;
    using UnityEngine;
    using UnityEngine.UI;
    using Singletons.Enums;

    public class ConnectStatusCardHandler : ConnectCardAbstract
    {
        [SerializeField]
        private Text m_Status;

        [SerializeField]
        private Image m_Loadbar;

        private ClientState m_KnownState;
        private ConnectTarget m_Target;

        private float m_LoadPercentageStep;
        private float m_LoadTarget;
        private float m_LoadPerc;
        private bool m_Loading;

        private bool m_Finishing = false;

        private const int SERVER_CONNECT_STEPS_DEFAULT = 5;
        private const int SERVER_CONNECT_STEPS_RECONNECT = 3;
        private const int LOBBY_CONNECT_STEPS = 2;

        private const float HIGHLIGHT_TIME = 4f;
        private const float HIGHLIGHT_SPEED = 4.5f;

        /// <summary>
        /// resets values related to loading bar
        /// </summary>
        private void ResetLoadMembers()
        {
            m_Loading = false;
            m_LoadTarget = 0;
            m_LoadPerc = 0;
            m_Loadbar.fillAmount = 0;
        }

        private void Update()
        {
            var currentState = PhotonNetwork.NetworkClientState;
            var statusUpdate = currentState != m_KnownState;
            if (statusUpdate)
            {
                m_KnownState = currentState;
                m_Status.text = StringUtils.AddWhiteSpaceAtUppers(m_KnownState.ToString());
                UpdateLoadTarget();
            }
            CheckForTaskFinished();
        }

        /// <summary>
        /// Updates load target based on load percentage step
        /// </summary>
        private void UpdateLoadTarget()
        {
            if (!m_Loading)
                return;

            m_LoadTarget += m_LoadPercentageStep;

            //make sure load target is is clamped to 1
            if (m_LoadTarget > 1f)
                m_LoadTarget = 1f;
        }

        private void CheckForTaskFinished()
        {
            if (!m_Loading && m_LoadPerc == 1f && !m_Finishing)
            {
                //if the loading bar is not loading and at its end, the task is finished (can be false)
                StartCoroutine(ShowTaskFinishedStatus(true, () => OnTaskFinished(m_Target)));
            }
        }

        /// <summary>
        /// Sets the target on which the card can base its loading
        /// </summary>
        /// <param name="target"></param>
        public void StartLoadWithTarget(ConnectTarget target)
        {
            switch (target)
            {
                case ConnectTarget.MasterDefault:
                    m_LoadPercentageStep = 1f / SERVER_CONNECT_STEPS_DEFAULT;
                    break;

                case ConnectTarget.MasterReconnect:
                    m_LoadPercentageStep = 1f / SERVER_CONNECT_STEPS_RECONNECT;
                    break;

                case ConnectTarget.Room:
                    break;
            }
            m_Target = target;

            StartCoroutine(LoadBarByStepPercentage());
        }

        /// <summary>
        /// Keeps loading the loadbar until it reaches 100%
        /// Increase the load target by using UpdateLoadTarget
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadBarByStepPercentage()
        {
            m_Loading = true;

            while (m_LoadPerc != 1f)
            {
                if (m_LoadPerc != m_LoadTarget)
                {
                    m_LoadPerc += Time.deltaTime;
                    if (m_LoadPerc > m_LoadTarget)
                        m_LoadPerc = m_LoadTarget;

                    m_Loadbar.fillAmount = Mathf.Lerp(0, 1, m_LoadPerc);
                }

                yield return new WaitForFixedUpdate();
            }

            m_Loading = false;
        }

        private IEnumerator ShowTaskFinishedStatus(bool succesfull, Action endAction)
        {
            m_Finishing = true;

            var cardBack = transform.Find("Back").GetComponent<Image>();
            var startColor = cardBack.color;
            var targetColor = succesfull ? Color.green : Color.red;
            var time = 0f;

            while (time < HIGHLIGHT_TIME)
            {
                time += Time.deltaTime * HIGHLIGHT_SPEED;
                cardBack.color = Color.Lerp(startColor, targetColor, Mathf.PingPong(time, 1f));
                yield return new WaitForFixedUpdate();
            }

            m_Finishing = false;

            endAction();
        }

        private void OnEnable()
        {
            m_KnownState = PhotonNetwork.NetworkClientState;
            m_Status.text = StringUtils.AddWhiteSpaceAtUppers(m_KnownState.ToString());
        }

        private void OnDisable()
        {
            ResetLoadMembers();
        }
    }
}