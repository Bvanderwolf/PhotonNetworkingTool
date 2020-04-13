namespace ConnectCards
{
    using ConnectCards.Enums;
    using Photon.Pun;
    using Photon.Realtime;
    using Singletons;
    using System;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using Utils;

    public class InRoomContentHandler : ContentHandler
    {
        [SerializeField]
        private Transform m_PlayerListContent;

        [SerializeField]
        private Transform m_ChatContent;

        [SerializeField]
        private Text m_ChatText;

        [SerializeField]
        private InputField m_ChatInput;

        [SerializeField]
        private CountDownHandler m_CountdownHandler;

        [SerializeField]
        private Button m_CountdownStopButton;

        private struct PlayerItem
        {
            public Text Name;
            public Text Status;
            public Selectable StatusSelectable;
            public Button SetMasterButton;
            public FocusAbleImage FocusAble;
            public GameObject GO;
        }

        //*should implement for each chatbot message enum a message
        private Func<string, string>[] m_ChatBotMessages = new Func<string, string>[]
        {
            (name) => name + " left the room",
            (name) => name + " joined the room",
            (name) => name + " is the new host",
            (name) => name + " is ready to play",
            (name) => name + " stopped countdown"
        };

        private PlayerItem[] m_PlayerItems;

        private const int CHAT_LINE_AMOUNT = 45;
        private const int CHAT_INPUT_CHAR_AMOUNT = 50;
        private int m_CurrentChatLine = 0;

        private const int DISABLE_STOP_COUNT = 3;

        private const string READYUP_TEXT = "Click To ReadyUp";

        public event Action<bool> ReadyStatusChanged;

        public event Action<string> ChatMessageHandled;

        public event Action<bool> CountdownStopped;

        public override void Init()
        {
            base.Init();

            m_PlayerItems = new PlayerItem[InRoomManager.MAX_PLAYERS_AMMOUNT];
            m_ChatInput.characterLimit = CHAT_INPUT_CHAR_AMOUNT;
            m_CountdownStopButton.onClick.AddListener(OnStopCountdownButtonClick);

            for (int i = 0; i < m_PlayerItems.Length; i++)
            {
                var child = m_PlayerListContent.GetChild(i);
                m_PlayerItems[i].Name = child.Find("Name").GetComponent<Text>();
                m_PlayerItems[i].Status = child.Find("Status").GetComponent<Text>();
                m_PlayerItems[i].StatusSelectable = m_PlayerItems[i].Status.GetComponent<Selectable>();
                m_PlayerItems[i].SetMasterButton = child.Find("SetMasterButton").GetComponent<Button>();
                m_PlayerItems[i].FocusAble = child.GetComponent<FocusAbleImage>();
                m_PlayerItems[i].FocusAble.AddListeners(OnPlayerItemSelected, OnPlayerItemDeselected);
                m_PlayerItems[i].GO = child.gameObject;
            }
            PlayerManager.Instance.SetInRoomStatus(InRoomStatus.Inactive);
        }

        public void SetActiveStateOfPlayerListContent(bool active)
        {
            if (!active)
            {
                foreach (var item in m_PlayerItems)
                {
                    item.SetMasterButton.onClick.RemoveAllListeners();
                    item.GO.SetActive(false);
                }
            }
            else
            {
                var players = PhotonNetwork.CurrentRoom.Players;
                var localPlayer = PhotonNetwork.LocalPlayer;
                var count = 0;
                foreach (var playerInfo in players)
                {
                    SetActiveStateOfPlayerItem(m_PlayerItems[count++], playerInfo.Value, localPlayer);
                }
            }
        }

        private void SetActiveStateOfPlayerItem(PlayerItem item, Player player, Player localPlayer)
        {
            item.Name.text = player.NickName;

            var isMyItem = player == localPlayer;
            var status = PlayerManager.Instance.GetInRoomStatus(player);
            var localInPlayerList = status == InRoomStatus.InPlayerlist && isMyItem;

            var statusString = localInPlayerList ? READYUP_TEXT : StringUtils.AddWhiteSpaceAtUppers(status.ToString());
            item.Status.text = statusString;
            item.StatusSelectable.interactable = isMyItem;

            var button = item.SetMasterButton;
            button.gameObject.SetActive(player.IsMasterClient);
            button.interactable = !player.IsMasterClient;

            if (localPlayer.IsMasterClient && !isMyItem)
            {
                button.onClick.AddListener(() =>
                {
                    OnSetMasterButtonClick(player);
                });
            }

            item.GO.SetActive(true);
        }

        private void OnPlayerItemSelected(GameObject selectedItem)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                var item = m_PlayerItems.Where(i => i.GO == selectedItem).First();
                if (item.Name.text != PhotonNetwork.NickName)
                    item.SetMasterButton.gameObject.SetActive(true);
            }
        }

        private void OnPlayerItemDeselected(GameObject deselectedItem)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                var item = m_PlayerItems.Where(i => i.GO == deselectedItem).First();
                if (item.Name.text != PhotonNetwork.NickName)
                    item.SetMasterButton.gameObject.SetActive(false);
            }
        }

        //should only be called when status selectable clicked is yours
        public void OnStatusSelectableClick(BaseEventData data)
        {
            if (data.selectedObject == null)
                return;

            var item = m_PlayerItems.Where(i => i.StatusSelectable.gameObject == data.selectedObject).First();
            var wasReady = PlayerManager.Instance.GetInRoomStatus(PhotonNetwork.LocalPlayer) == InRoomStatus.Ready;
            //if the player wasn't ready when clicking, he is now ready
            UpdateRoomStatus(item, wasReady ? InRoomStatus.InPlayerlist : InRoomStatus.Ready);
            ReadyStatusChanged(!wasReady);
        }

        private void UpdateRoomStatus(PlayerItem item, InRoomStatus status)
        {
            var localInPlayerList = status == InRoomStatus.InPlayerlist && item.Name.text == PhotonNetwork.NickName;
            item.Status.text = localInPlayerList ? READYUP_TEXT : status.ToString();
            PlayerManager.Instance.SetInRoomStatus(status);
        }

        private void OnSetMasterButtonClick(Player player)
        {
            if (PhotonNetwork.IsMasterClient && player != PhotonNetwork.LocalPlayer)
                PhotonNetwork.SetMasterClient(player);
        }

        private void OnStopCountdownButtonClick()
        {
            if (m_CountdownHandler.CountingDown)
            {
                CountdownStopped(false);
            }
        }

        public void SetActiveStateOfChatContent(bool value)
        {
            m_ChatContent.gameObject.SetActive(value);
        }

        public void HandlePlayerChatInput(string input)
        {
            ClearChatInput();
            ResetFocusOnChatInput();

            if (IsClearInput(input))
            {
                ClearChat();
                return;
            }

            ChatMessageHandled(input);
        }

        private void ResetFocusOnChatInput()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem.currentSelectedGameObject == m_ChatInput.gameObject)
                m_ChatInput.OnPointerClick(new PointerEventData(eventSystem));
        }

        private void ClearChatInput()
        {
            m_ChatInput.text = "";
        }

        private void ClearChat()
        {
            m_ChatText.text = "";
            m_CurrentChatLine = 0;
        }

        private void TrimFirstLineOfChat()
        {
            var text = m_ChatText.text;
            //grab index of first newline from where i can start the substring
            var indexOfNewLine = text.IndexOf("\n") + 1;
            m_ChatText.text = text.Substring(indexOfNewLine);
        }

        public void AddTextToChat(string text, Player sender)
        {
            if (m_CurrentChatLine != CHAT_LINE_AMOUNT)
                m_CurrentChatLine++;
            else
                TrimFirstLineOfChat();

            var time = string.Format("({0:t}) ", DateTime.Now);
            var user = string.Format("({0})", sender.NickName);
            m_ChatText.text += (time + user + ": " + text + "\n");
        }

        public void AddTextToChat(ChatBotMessages message, string nameOfPlayer)
        {
            if (m_CurrentChatLine != CHAT_LINE_AMOUNT)
                m_CurrentChatLine++;
            else
                TrimFirstLineOfChat();

            var time = string.Format("({0:t})", DateTime.UtcNow);
            var text = m_ChatBotMessages[(int)message];
            m_ChatText.text += (time + ": " + text(nameOfPlayer) + "\n");
        }

        private bool IsClearInput(string input)
        {
            return input == "/Clear" || input == "/clear";
        }

        public void SetActiveStatusOfCountdownContent(bool active)
        {
            if (active)
            {
                m_CountdownHandler.gameObject.SetActive(true);
            }
            else
            {
                m_CountdownHandler.StopCountdown();
                m_CountdownHandler.gameObject.SetActive(false);
                m_CountdownStopButton.interactable = true;
            }
        }

        public void StartGameCountdown()
        {
            m_CountdownHandler.StartCountDown(() =>
            {
                //countdown stopped at zero (succesfull)
                CountdownStopped(true);
            },
            (count) =>
            {
                //check each count if stop button can be disabled
                if (count == DISABLE_STOP_COUNT)
                    m_CountdownStopButton.interactable = false;
            });
        }

        public void OnInRoomStatusChange(Player player, InRoomStatus status)
        {
            SetActiveStateOfPlayerListContent(false);
            SetActiveStateOfPlayerListContent(true);

            if (status == InRoomStatus.Ready)
                AddTextToChat(ChatBotMessages.IsReady, player.NickName);
        }

        public void OnMasterClientChange(Player newMaster)
        {
            SetActiveStateOfPlayerListContent(false);
            SetActiveStateOfPlayerListContent(true);
            AddTextToChat(ChatBotMessages.NewMasterClient, newMaster.NickName);
        }

        public void OnPlayerEnteredRoom(Player player)
        {
            SetActiveStateOfPlayerListContent(false);
            SetActiveStateOfPlayerListContent(true);
            AddTextToChat(ChatBotMessages.PlayerJoined, player.NickName);
        }

        public void OnPlayerLeftRoom(Player player)
        {
            SetActiveStateOfPlayerListContent(false);
            SetActiveStateOfPlayerListContent(true);
            AddTextToChat(ChatBotMessages.PlayerLeft, player.NickName);
        }
    }
}