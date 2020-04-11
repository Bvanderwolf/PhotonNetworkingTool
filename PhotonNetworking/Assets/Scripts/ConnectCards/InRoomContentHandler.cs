namespace ConnectCards
{
    using Photon.Pun;
    using Photon.Realtime;
    using Singletons;
    using UnityEngine;
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
            public Button SetMasterButton;
            public GameObject GO;
        }

        private enum InRoomStatus
        {
            Inactive,
            InPlayerlist,
            Chatting,
            Ready
        }

        private PlayerItem[] m_PlayerItems;

        public override void Init()
        {
            base.Init();

            m_PlayerItems = new PlayerItem[InRoomManager.MAX_PLAYERS_AMMOUNT];

            for (int i = 0; i < m_PlayerItems.Length; i++)
            {
                var item = m_PlayerListContent.GetChild(i);
                m_PlayerItems[i].Name = item.Find("Name").GetComponent<Text>();
                m_PlayerItems[i].Status = item.Find("Status").GetComponent<Text>();
                m_PlayerItems[i].SetMasterButton = item.Find("SetMasterButton").GetComponent<Button>();
                m_PlayerItems[i].GO = item.gameObject;
            }

            PlayerManager.Instance.UpdateProperties<InRoomStatus>("InRoomStatus", InRoomStatus.Inactive);
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

            var status = (InRoomStatus)player.CustomProperties["InRoomStatus"];
            var statusString = StringUtils.AddWhiteSpaceAtUppers(status.ToString());
            item.Status.text = statusString;

            var button = item.SetMasterButton;
            button.gameObject.SetActive(player.IsMasterClient);

            if (localPlayer.IsMasterClient && player != localPlayer)
            {
                button.onClick.AddListener(() =>
                {
                    OnSetMasterButtonClick(player);
                });
            }

            item.GO.SetActive(true);
        }

        public void OnSetMasterButtonClick(Player player)
        {
            if (PhotonNetwork.IsMasterClient && player != PhotonNetwork.LocalPlayer)
                PhotonNetwork.SetMasterClient(player);
        }

        public void SetActiveStateOfChatContent(bool value)
        {
        }

        public void OnPlayerEnteredRoom(Player player)
        {
            print("player entered");
        }

        public void OnPlayerLeftRoom(Player player)
        {
            print("player left");
        }
    }
}