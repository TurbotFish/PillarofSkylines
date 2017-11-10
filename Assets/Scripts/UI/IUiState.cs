namespace Game.UI {
    public interface IUiState
    {
        bool IsActive { get; }

        void Initialize(Player.PlayerModel playerModel);

        void Activate(Utilities.EventManager.OnShowMenuEventArgs args);

        void Deactivate();
    }
}