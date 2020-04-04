using UnityEngine;
using UnityEngine.UI;

public class DisconnectCardHandler : ConnectCardAbstract
{
    [SerializeField]
    private Button m_ReconnectButton;

    [SerializeField]
    private Text m_Cause;

    private void Awake()
    {
        m_ReconnectButton.onClick.AddListener(() =>
        {
            OnTaskFinished(null);
        });
    }

    public void SetCause(string cause)
    {
        m_Cause.text = $"Cause: {cause}";
    }
}
