namespace ConnectCards
{
    using ConnectCards.HelperStructs;
    using Singletons.Enums;

    public interface ISimpleConnectCardInteractable
    {
        void SetActiveAtTarget(ConnectTarget target);

        ConnectCardHandlerInfo GetInfo();
    }
}