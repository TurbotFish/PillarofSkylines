using Game.GameControl;
using Game.Model;
using Game.Utilities;
using Game.World;
using System;

namespace Game.LevelElements
{
    public abstract class PersistentLevelElement<T> : UniqueIdOwner, IWorldObject where T : PersistentData
    {
        //##################################################################

        private PersistentData persistentData;
        private IGameControllerBase gameController;

        private bool isCopy;
        private bool isInitialized;

        //##################################################################

        #region initialization methods

        public virtual void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            this.gameController = gameController;
            this.isCopy = isCopy;

            persistentData = gameController.PlayerModel.GetPersistentDataObject(UniqueId);

            if (persistentData == null)
            {
                persistentData = CreatePersistentDataObject();
                gameController.PlayerModel.AddPersistentDataObject(persistentData);
            }

            isInitialized = true;
        }

        #endregion initialization methods

        //##################################################################

        #region inquiries

        public bool IsCopy { get { return isCopy; } }
        public bool IsInitialized { get { return isInitialized; } }

        protected IGameControllerBase GameController { get { return gameController; } }
        protected T PersistentData { get { return persistentData as T; } }

        #endregion inquiries

        //##################################################################

        #region operations

        protected abstract PersistentData CreatePersistentDataObject();

        #endregion operations

        //##################################################################
    }
} // end of namespace