using UnityEngine;
using UnityEngine.UI;

public class DisconnectCardHandler : ConnectCardAbstract
{
    [SerializeField]
    private Button m_ReconnectButton;

    [SerializeField]
    private Text m_Cause;

    [SerializeField]
    private KeyCode ReconnectKey = KeyCode.Return;

    public override void Init()
    {
        base.Init();
        m_ReconnectButton.onClick.AddListener(() =>
        {
            OnTaskFinished(null);
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(ReconnectKey))
        {
            OnTaskFinished(null);
        }
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