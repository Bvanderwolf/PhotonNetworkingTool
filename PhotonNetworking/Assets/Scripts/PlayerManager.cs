using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

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