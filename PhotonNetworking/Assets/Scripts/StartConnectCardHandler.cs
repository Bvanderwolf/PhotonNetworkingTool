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
    private Button m_SelectionButton;

    [SerializeField]
    private Button m_GenericButton;

    [SerializeField]
    private KeyCode ConnectKey = KeyCode.Return;

    private Animator m_Animator;
    private bool m_Animating = false;

    private bool m_NicknameInputed = false;
    public const string GENERIC_CONNECT_NAME = "Player";

    public override void Init()
    {
        base.Init();
        m_NickNameInput.onEndEdit.AddListener(OnNickNameInputFinished);
        m_SelectionButton.onClick.AddListener(OnSelectionButtonClick);
        m_GenericButton.onClick.AddListener(OnGenericNameButtonClick);
        m_Animator = m_NickNameInput.GetComponent<Animator>();

        m_NickNameInput.characterLimit = m_MaxNicknameLength;
    }

    public void OnNickNameInputFinished(string nickname)
    {
        if (nickname.Length == 0)
        {
            m_NicknameInputed = false;
            GiveFaulthyNicknameWarning();
        }
        else
        {
            m_NicknameInputed = true;
        }
    }

    private void OnGenericNameButtonClick()
    {
        OnTaskFinished(GENERIC_CONNECT_NAME);
    }

    public void OnSelectionButtonClick()
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