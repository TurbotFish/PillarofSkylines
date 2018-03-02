namespace Game.World
{
    public interface IWorldObject
    {
        void Initialize(GameControl.IGameControllerBase gameController, bool isCopy);
    }
} //end of namespace