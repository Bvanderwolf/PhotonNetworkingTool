using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperTools : MonoBehaviour, IDeveloperCallbacks
{
    [SerializeField] private Button m_FullscreenButton;
    [SerializeField] private Dropdown m_ScreenResolutions;
    [SerializeField] private Button m_DisconnectButton;
    [SerializeField] private Toggle m_StastGUIToggle;
    [SerializeField] private Toggle m_LagSimGUIToggle;
    [SerializeField] private Text m_PlayerNickname;
    [SerializeField] private Text m_CurrentClientState;
    [SerializeField] private Image m_LobbyStatUpdate;
    [SerializeField] private Image m_AppStateUpdate;
    [SerializeField] private AppStats m_AppInfo;

    private int m_ScreenWidth;
    private int m_ScreenHeight;

    //According to Photon documentation lobby stat updates occur every minute (seems to be 2 after testing)
    private const float LOBBY_STAT_UPDATE_INTERVAL = 120f;

    //According to Photon documentation the app's stats update every 5 seconds
    private const float APP_STATS_UPDATE_INTERVAL = 5f;

    private float m_TimeTilLastLobbyStatUpdate = 0;
    private float m_TimeTilLastAppStatUpdate = 0;
    private bool m_ConnectedToMaster = false;

    private ClientState shownState;

    private Resolution m_FullScreenResolution;

    [System.Serializable]
    private struct AppStats
    {
        public Text CountOfRooms;
        public Text CountOfPlayersOnMaster;
        public Text CountOfPlayersInRooms;
        public Text CountOfPlayers;
    }

    private PhotonStatsGui m_StatsGUI;
    private PhotonLagSimulationGui m_LaggSimulationGUI;

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

        m_StastGUIToggle.onValueChanged.AddListener(OnStatsGuiToggleClick);
        m_LagSimGUIToggle.onValueChanged.AddListener(OnLagSimGuiToggleClick);

        m_StatsGUI = GetComponent<PhotonStatsGui>();
        m_LaggSimulationGUI = GetComponent<PhotonLagSimulationGui>();

        UpdateClientStateText(shownState);
    }

    private void Start()
    {
        ConnectionManager.Instance.AddCallbackTarget(this);
        InLobbyManager.Instance.AddCallbackTarget(this);

        ToggleActiveSelf();
    }

    private void OnDestroy()
    {
        ConnectionManager.Instance.RemoveCallbackTarget(this);
        InLobbyManager.Instance.RemoveCallbackTarget(this);
    }

    private void FixedUpdate()
    {
        var state = PhotonNetwork.NetworkClientState;
        if (state != shownState)
        {
            UpdateClientStateText(state);
        }

        if (m_ConnectedToMaster && !PhotonNetwork.InRoom)
        {
            m_TimeTilLastAppStatUpdate += Time.deltaTime;
            if (m_TimeTilLastAppStatUpdate > APP_STATS_UPDATE_INTERVAL)
            {
                UpdateApplicationStatistics();
                m_TimeTilLastAppStatUpdate = 0;
            }
            m_AppStateUpdate.fillAmount = 1 - (m_TimeTilLastAppStatUpdate / APP_STATS_UPDATE_INTERVAL);

            m_TimeTilLastLobbyStatUpdate += Time.deltaTime;
            m_TimeTilLastLobbyStatUpdate = Mathf.Clamp(m_TimeTilLastLobbyStatUpdate, 0, LOBBY_STAT_UPDATE_INTERVAL);
            m_LobbyStatUpdate.fillAmount = 1 - (m_TimeTilLastLobbyStatUpdate / LOBBY_STAT_UPDATE_INTERVAL);
        }
    }

    public void ToggleActiveSelf()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    private void ResetStatisticsUpdate()
    {
        m_TimeTilLastAppStatUpdate = 0;
        m_TimeTilLastLobbyStatUpdate = 0;
        m_AppStateUpdate.fillAmount = 1;
        m_LobbyStatUpdate.fillAmount = 1;
    }

    private void UpdateClientStateText(ClientState state)
    {
        var stateString = StringUtils.AddWhiteSpaceAtUppers(state.ToString());
        m_CurrentClientState.text = "Client State: " + stateString;
    }

    private void UpdateApplicationStatistics()
    {
        m_AppInfo.CountOfRooms.text = "Total Room Count: " + PhotonNetwork.CountOfRooms;
        m_AppInfo.CountOfPlayersOnMaster.text = "Players On Master: " + PhotonNetwork.CountOfPlayersOnMaster;
        m_AppInfo.CountOfPlayersInRooms.text = "Players In Rooms: " + PhotonNetwork.CountOfPlayersInRooms;
        m_AppInfo.CountOfPlayers.text = "Total Player Count: " + PhotonNetwork.CountOfPlayers;
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

    private void OnStatsGuiToggleClick(bool newValue)
    {
        m_StatsGUI.statsWindowOn = newValue;
        m_StatsGUI.statsOn = newValue;
    }

    private void OnLagSimGuiToggleClick(bool newValue)
    {
        m_LaggSimulationGUI.Visible = newValue;
    }

    private void DisconnectFromServer()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
    }

    public void OnConnected()
    {
        m_DisconnectButton.interactable = true;
    }

    public void OnJoinedRoom()
    {
        ResetStatisticsUpdate();
    }

    public void OnConnectedToMaster()
    {
        m_ConnectedToMaster = true;
    }

    public void OnDisconnected()
    {
        m_DisconnectButton.interactable = false;
        m_ConnectedToMaster = false;

        ResetStatisticsUpdate();
    }

    public void OnPlayerNickNameUpdate(string newNickname)
    {
        m_PlayerNickname.text = "Nickname: " + newNickname;
    }

    public void OnLobbyStatisticsUpdate()
    {
        print(m_TimeTilLastLobbyStatUpdate);
        m_TimeTilLastLobbyStatUpdate = 0;
    }
}