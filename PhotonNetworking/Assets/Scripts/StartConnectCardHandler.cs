using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartConnectCardHandler : ConnectCardAbstract
{
    [SerializeField]
    private int m_MaxNicknameLength = 10;

    [SerializeField]
    private InputField m_NickNameInput;

    [SerializeField]
    private Button m_ConnectButton;

    private Animator m_Animator;
    private bool m_Animating = false;

    private bool m_NicknameInputed = false;

    private void Awake()
    {
        m_NickNameInput.onEndEdit.AddListener(OnNickNameInputFinished);
        m_ConnectButton.onClick.AddListener(OnConnectButtonClick);
        m_Animator = m_NickNameInput.GetComponent<Animator>();

        m_NickNameInput.characterLimit = m_MaxNicknameLength;
    }

    public void OnNickNameInputFinished(string nickname)
    {
        if(nickname.Length == 0)
        {
            m_NicknameInputed = false;
            GiveFaulthyNicknameWarning();          
        }
        else
        {
            m_NicknameInputed = true;
        }
    }

    public void OnConnectButtonClick()
    {
        if (!m_NicknameInputed)
        {
            GiveFaulthyNicknameWarning();
            return;
        }
        OnTaskFinished(m_NickNameInput.text);
    }

    private void GiveFaulthyNicknameWarning()
    {
        if (!m_Animating)
        {
            m_Animator.SetTrigger("Warn");
            StartCoroutine(WaitForAnimation());
        }
        
    }

    private IEnumerator WaitForAnimation()
    {
        float secs = m_Animator.runtimeAnimatorController.animationClips[0].length;
        m_Animating = true;
        yield return new WaitForSeconds(secs);
        m_Animating = false;
    }
}
