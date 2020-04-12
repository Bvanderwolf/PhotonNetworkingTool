namespace Singletons
{
    using Utils;
    using Photon.Pun;
    using UnityEngine;
    using Photon.Realtime;
    using System.Linq;

    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }

        public const string GENERIC_CONNECT_NAME = "Player";
        public const string GENERIC_NICKNAME_KEY = "GenericNickname";

        private IDeveloperCallbacks m_DeveloperTarget = null;

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(this.gameObject);
        }

        public void AddCallbackTarget(object target)
        {
            if (target as IDeveloperCallbacks != null)
            {
                if (m_DeveloperTarget == null)
                    m_DeveloperTarget = (IDeveloperCallbacks)target;
            }
        }

        public void RemoveCallbackTarget(object target)
        {
            if (target as IDeveloperCallbacks != null)
            {
                if (m_DeveloperTarget != null)
                    m_DeveloperTarget = null;
            }
        }

        public void UpdateNickname(string nickname)
        {
            if (string.IsNullOrEmpty(nickname))
                return;

            PhotonNetwork.NickName = nickname;
            m_DeveloperTarget.OnPlayerNickNameUpdate(nickname);
        }

        public void SetHasGenericNickname(string nickname)
        {
            var isGeneric = nickname == GENERIC_CONNECT_NAME;
            UpdateProperties<bool>(GENERIC_NICKNAME_KEY, isGeneric);
        }

        public bool HasGenericNickname()
        {
            var player = PhotonNetwork.LocalPlayer;
            return player.NickName == GENERIC_CONNECT_NAME || (bool)player.CustomProperties[GENERIC_NICKNAME_KEY];
        }

        public void OnCreatingRoom()
        {
            if (HasGenericNickname())
            {
                UpdateNickname(GENERIC_CONNECT_NAME + "1");
            }
        }

        public void OnJoiningRoom(RoomInfo room)
        {
            if (HasGenericNickname())
            {
                UpdateNickname(GENERIC_CONNECT_NAME + (room.PlayerCount + 1));
            }
        }

        public void OnJoinedRoom()
        {
            if (HasGenericNickname())
            {
                var name = PhotonNetwork.NickName;
                var actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
                if (PhotonNetwork.CurrentRoom.Players.Any(p => p.Value.ActorNumber != actorNum && p.Value.NickName == name))
                {
                    var newNickname = GENERIC_CONNECT_NAME + actorNum;
                    UpdateNickname(newNickname);
                }
            }
        }

        /// <summary>
        /// Updates the local player its custom properties with T being
        /// the type of value that goes with the given key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void UpdateProperties<T>(string key, object value)
        {
            var properties = PhotonNetwork.LocalPlayer.CustomProperties;
            if (properties.ContainsKey(key))
            {
                properties[key] = (T)value;
            }
            else
            {
                properties.Add(key, (T)value);
            }
            PhotonNetwork.SetPlayerCustomProperties(properties);
        }

        public object GetProperty(string key)
        {
            var properties = PhotonNetwork.LocalPlayer.CustomProperties;
            return properties.ContainsKey(key) ? properties[key] : null;
        }
    }
}