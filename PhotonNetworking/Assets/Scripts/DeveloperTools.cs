using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperTools : MonoBehaviour, IDeveloperCallbacks
{
    [SerializeField] private Button m_FullscreenButton;
    [SerializeField] private Dropdown m_ScreenResolutions;
    [SerializeField] private Button m_DisconnectButton;
    [SerializeField] private Text m_PlayerNickname;
    [SerializeField] private Text m_CurrentClientState;
    [SerializeField] private Image m_LobbyStatUpdate;
    [SerializeField] private Image m_AppStateUpdate;

    private int m_ScreenWidth;
    private int m_ScreenHeight;

    private Resolution m_FullScreenResolution;

    private void Awake()
    {
        if (Application.isEditor)
        {
            m_FullscreenButton.interactable = false;
            m_ScreenResolutions.interactable = false;
        }
        else
        {
            m_ScreenWidth = Screen.width;
            m_ScreenHeight = Screen.height;
            m_FullScreenResolution = Screen.currentResolution;

            PlayerPrefs.SetInt("Screenmanager Resolution Width", m_ScreenWidth);
            PlayerPrefs.SetInt("Screenmanager Resolution Height", m_ScreenHeight);
            PlayerPrefs.SetInt("Screenmanager Is Fullscreen mode", Screen.fullScreen ? 1 : 0);

            m_FullscreenButton.onClick.AddListener(OnFullScreenButtonClick);
            m_ScreenResolutions.onValueChanged.AddListener(OnResolutionChange);
            SetStartValueOfScreenResolutions();
        }
        m_DisconnectButton.onClick.AddListener(DisconnectFromServer);
        m_DisconnectButton.interactable = false;
    }

    private void Start()
    {
        ConnectionManager.Instance.AddCallbackTarget(this);
        InLobbyManager.Instance.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        ConnectionManager.Instance.RemoveCallbackTarget(this);
        InLobbyManager.Instance.RemoveCallbackTarget(this);
    }

    private void SetStartValueOfScreenResolutions()
    {
        var windowResolution = $"{Screen.width}x{Screen.height}";
        var index = 0;
        foreach (var option in m_ScreenResolutions.options)
        {
            if (option.text == windowResolution)
                break;
            index++;
        }
        m_ScreenResolutions.SetValueWithoutNotify(index);
    }

    private void DisconnectFromServer()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
    }

    //----------------------------------------------------------------------------------------------//
    // Note* Static Screen members are updated one render frame (FixedUpdate) after resolution set //
    //--------------------------------------------------------------------------------------------//
    private void OnFullScreenButtonClick()
    {
        SetFullScreen(!Screen.fullScreen);
    }

    private void SetFullScreen(bool value)
    {
        Screen.SetResolution(m_ScreenWidth, m_ScreenHeight, value);
        PlayerPrefs.SetInt("Screenmanager Is Fullscreen mode", value ? 1 : 0);
    }

    private bool OnFullScreenResolution()
    {
        var displayResolution = Screen.currentResolution;
        return displayResolution.width == Screen.width && displayResolution.height == Screen.height;
    }

    private void OnResolutionChange(int dropdownIndex)
    {
        var newCaption = m_ScreenResolutions.captionText.text;
        var resolutionValues = newCaption.Split('x');

        m_ScreenWidth = int.Parse(resolutionValues[0]);
        m_ScreenHeight = int.Parse(resolutionValues[1]);

        PlayerPrefs.SetInt("Screenmanager Resolution Width", m_ScreenWidth);
        PlayerPrefs.SetInt("Screenmanager Resolution Height", m_ScreenHeight);

        if (OnFullScreenResolution() && Screen.fullScreen)
        {
            /*if the window covers the screen and has full screen on, the new
             resolution doesn't use full screen mode*/
            Screen.SetResolution(m_ScreenWidth, m_ScreenHeight, false);
        }
        else
        {
            Screen.SetResolution(m_ScreenWidth, m_ScreenHeight, Screen.fullScreen);
        }
    }

    public void OnConnected()
    {
        m_DisconnectButton.interactable = true;
    }

    public void OnConnectedToMaster()
    {
    }

    public void OnDisconnected()
    {
        m_DisconnectButton.interactable = false;
    }

    public void OnPlayerNickNameUpdate(string newNickname)
    {
    }

    public void OnLobbyStatisticsUpdate()
    {
    }
}