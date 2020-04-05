using UnityEngine;
using UnityEngine.UI;

public class DisconnectCardHandler : ConnectCardAbstract
{
    [SerializeField]
    private Button m_ReconnectButton;

    [SerializeField]
    private Text m_Cause;

    public override void Init()
    {
        base.Init();
        m_ReconnectButton.onClick.AddListener(() =>
        {
            OnTaskFinished(null);
        });
    }

    /// <summary>
    /// Display given cause on card
    /// </summary>
    /// <param name="cause"></param>
    public void SetCause(string cause)
    {
        m_Cause.text = $"Cause: {cause}";
    }
}
