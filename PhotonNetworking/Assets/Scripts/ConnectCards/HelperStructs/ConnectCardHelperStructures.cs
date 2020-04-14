namespace ConnectCards.HelperStructs
{
    using ConnectCards.Enums;
    using UnityEngine.SceneManagement;

    public struct InLobbyConnectResult
    {
        public object args;
        public InLobbyConnectChoice choice;
    }

    public struct CreateRoomFormResult
    {
        public string Name;
        public int MaxPlayers;
        public bool IsVisible;
        public bool IsOpen;
    }

    public struct ConnectCardJobResult
    {
        public Scene SceneLoaded;
        public LoadSceneMode Mode;
        public ISimpleConnectCardInteractable Interactable;
        public ConnectCardHandlerInfo info;
    }

    public struct ConnectCardHandlerInfo
    {
    }
}