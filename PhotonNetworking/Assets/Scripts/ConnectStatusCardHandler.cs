using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ConnectStatusCardHandler : ConnectCardAbstract
{
    [SerializeField]
    private Text m_Status;

    [SerializeField]
    private Image m_Loadbar;

    private ClientState m_KnownState;

    private float m_LoadPercentageStep;
    private float m_LoadTarget;
    private float m_LoadStartPerc;
    private float m_LoadPerc;
    private bool m_Loading;

    private const int SERVER_CONNECT_STEPS = 4;

    private void Awake()
    {
        m_KnownState = PhotonNetwork.NetworkClientState;
        m_Status.text = GetFormattedStatus(m_KnownState.ToString());
    }

    private void ResetLoadMembers()
    {
        m_Loading = false;
        m_LoadTarget = 0;
        m_LoadStartPerc = 0;
        m_LoadPerc = 0;
        m_Loadbar.fillAmount = 0;
    }

    private void Update()
    {
        var currentState = PhotonNetwork.NetworkClientState;
        var statusUpdate = currentState !=  m_KnownState;
        if (statusUpdate)
        {
            m_KnownState = PhotonNetwork.NetworkClientState;
            m_Status.text = GetFormattedStatus(m_KnownState.ToString());
            UpdateLoadBar();
        }
    }

    private void UpdateLoadBar()
    {
        if (m_Loading)
        {
            //set lerp start to current fill ammount
            m_LoadStartPerc = m_Loadbar.fillAmount;  
            //set target to target plus another step
            m_LoadTarget += m_LoadPercentageStep;
            //reset current load percentage
            m_LoadPerc = 0;
        }
        else
        {
            StartCoroutine(LoadBarByStepPercentage());
        }       
    }

    private void CheckForTaskFinished()
    {
        if(!m_Loading && m_LoadPerc == 1f)
        {
            ResetLoadMembers();
            OnTaskFinished(null);
        }
    }

    public void SetConnectTarget(ConnectTarget target)
    {
        switch (target)
        {
            case ConnectTarget.MASTER:
                m_LoadPercentageStep = 1f / SERVER_CONNECT_STEPS;
                break;
            case ConnectTarget.LOBBY:
                break;
            case ConnectTarget.ROOM:
                break;
        }
    }

    private string GetFormattedStatus(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
            return "";

        StringBuilder newText = new StringBuilder(state.Length * 2);
        newText.Append(state[0]);
        for (int i = 1; i < state.Length; i++)
        {
            if (char.IsUpper(state[i]) && state[i - 1] != ' ')
                newText.Append(' ');
            newText.Append(state[i]);
        }

        return newText.ToString();
    }

    private IEnumerator LoadBarByStepPercentage()
    {
        m_Loading = true;
        m_LoadStartPerc = m_Loadbar.fillAmount;
        m_LoadTarget = m_Loadbar.fillAmount + m_LoadPercentageStep;  
        
        while(m_LoadPerc < m_LoadTarget)
        {
            if (m_LoadPerc > 1f)
                m_LoadPerc = 1f;

            m_LoadPerc += Time.deltaTime;
            m_Loadbar.fillAmount = Mathf.Lerp(m_LoadStartPerc, m_LoadTarget, m_LoadPerc);
            yield return new WaitForFixedUpdate();
        }

        m_Loading = false;
    }
}
