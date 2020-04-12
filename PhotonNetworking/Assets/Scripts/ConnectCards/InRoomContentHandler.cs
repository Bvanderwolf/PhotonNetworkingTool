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

        private struct PlayerItem
        {
            public Text Name;
            public Text Status;
            public Selectable StatusSelectable;
            public Button SetMasterButton;
            public FocusAbleImage FocusAble;
            public GameObject GO;
        }

        private enum ChatBotMessages
        {
            PlayerLeft,
            PlayerJoined,
            NewMasterClient,
            IsReady
        }

        //*should implement for each chatbot message enum a message
        private Func<string, string>[] m_ChatBotMessages = new Func<string, string>[]
        {
            (name) => name + " left the room",
            (name) => name + " joined the room",
            (name) => name + " is the new host",
            (name) => name + " is ready to play"
        };

        private PlayerItem[] m_PlayerItems;

        private PhotonView m_PV;

        private const int CHAT_LINE_AMOUNT = 45;
        private const int CHAT_INPUT_CHAR_AMOUNT = 50;
        private int m_CurrentChatLine = 0;

        public override void Init()
        {
            base.Init();

            m_PlayerItems = new PlayerItem[InRoomManager.MAX_PLAYERS_AMMOUNT];
            m_PV = GetComponent<PhotonView>();
            m_ChatInput.characterLimit = CHAT_INPUT_CHAR_AMOUNT;

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

            var status = PlayerManager.Instance.GetInRoomStatus(player);
            var statusString = StringUtils.AddWhiteSpaceAtUppers(status.ToString());
            item.Status.text = statusString;

            var isMyItem = player == localPlayer;
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
            var item = m_PlayerItems.Where(i => i.StatusSelectable.gameObject == data.selectedObject).First();
            var isReady = PlayerManager.Instance.GetInRoomStatus(PhotonNetwork.LocalPlayer) == InRoomStatus.Ready;
            UpdateRoomStatus(item, isReady ? InRoomStatus.InPlayerlist : InRoomStatus.Ready);

            if (!isReady)
            {
                //if the player wasn't ready when clicking, he is now ready and must wait on other players
                //*disable interactability of playerlist and chat button
            }
        }

        private void UpdateRoomStatus(PlayerItem item, InRoomStatus status)
        {
            item.Status.text = status.ToString();
            PlayerManager.Instance.SetInRoomStatus(status);
        }

        public void OnSetMasterButtonClick(Player player)
        {
            if (PhotonNetwork.IsMasterClient && player != PhotonNetwork.LocalPlayer)
                PhotonNetwork.SetMasterClient(player);
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

            m_PV.RPC("UpdateChatWithMessage", RpcTarget.AllViaServer, input, PhotonNetwork.LocalPlayer);
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

        private void AddTextToChat(string text, Player sender)
        {
            if (m_CurrentChatLine != CHAT_LINE_AMOUNT)
                m_CurrentChatLine++;
            else
                TrimFirstLineOfChat();

            var time = string.Format("({0:t}) ", DateTime.Now);
            var user = string.Format("({0})", sender.NickName);
            m_ChatText.text += (time + user + ": " + text + "\n");
        }

        private void AddTextToChat(ChatBotMessages message, string nameOfPlayer)
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

        [PunRPC]
        private void UpdateChatWithMessage(string message, Player sender)
        {
            AddTextToChat(message, sender);
        }
    }
}