using Game.GameControl;

namespace Game.UI
{
    public interface IUiState
    {
        bool IsActive { get; }

        void Initialize(IGameController gameController);

        void Activate(Utilities.EventManager.OnShowMenuEventArgs args);

        void Deactivate();
    }
}