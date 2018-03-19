namespace Game.World
{
    public interface IWorldObject
    {
        void Initialize(GameControl.IGameControllerBase gameController, bool isCopy);
    }

    public interface IWorldObjectDuplication
    {
        void OnDuplication();
    }
} //end of namespace