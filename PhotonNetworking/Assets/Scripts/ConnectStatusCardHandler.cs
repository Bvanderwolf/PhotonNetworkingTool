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
    private float m_LoadPerc;
    private bool m_Loading;

    private const int SERVER_CONNECT_STEPS_DEFAULT = 5;
    private const int SERVER_CONNECT_STEPS_RECONNECT = 3;

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
            m_Status.text = GetFormattedStatus(m_KnownState.ToString());
            UpdateLoadBar();            
        }
        CheckForTaskFinished();
    }

    private void UpdateLoadBar()
    {
        m_LoadTarget += m_LoadPercentageStep;

        if (m_LoadTarget > 1f)
            m_LoadTarget = 1f;
    }

    private void CheckForTaskFinished()
    {
        if(!m_Loading && m_LoadPerc == 1f)
        {           
            OnTaskFinished(null);
        }
    }

    public void Setup(ConnectTarget target)
    {
        switch (target)
        {
            case ConnectTarget.MasterDefault:
                m_LoadPercentageStep = 1f / SERVER_CONNECT_STEPS_DEFAULT;
                break;
            case ConnectTarget.MasterReconnect:
                m_LoadPercentageStep =  1f / SERVER_CONNECT_STEPS_RECONNECT;
                break;
            case ConnectTarget.Lobby:
                break;
            case ConnectTarget.Room:
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
        
        while(m_LoadPerc != 1f)
        {
            if(m_LoadPerc != m_LoadTarget)
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

    private void OnEnable()
    {
        m_KnownState = PhotonNetwork.NetworkClientState;
        m_Status.text = GetFormattedStatus(m_KnownState.ToString());

        StartCoroutine(LoadBarByStepPercentage());
    }

    private void OnDisable()
    {
        ResetLoadMembers();
    }
}
