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

        protected PersistentData persistentData;
        protected IGameControllerBase gameController;

        private bool isCopy;
        private bool isInitialized;

        //##################################################################

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

            if (!isInitialized)
            {
                PersistentData.OnPersistentDataChange += OnPersistentDataChange; // If the instance has already been initialized it subscribes to the event in OnEnable.
            }

            isInitialized = true;
        }

        //##################################################################

        public bool IsCopy { get { return isCopy; } }
        public bool IsInitialized { get { return isInitialized; } }
        protected T PersistentData { get { return persistentData as T; } }

        //##################################################################

        protected abstract void OnPersistentDataChange(object sender, EventArgs args);

        protected abstract PersistentData CreatePersistentDataObject();

        protected virtual void OnEnable()
        {
            if (isInitialized)
            {
                PersistentData.OnPersistentDataChange += OnPersistentDataChange;
            }
        }

        protected virtual void OnDisable()
        {
            if (isInitialized)
            {
                PersistentData.OnPersistentDataChange -= OnPersistentDataChange;
            }
        }

        //##################################################################
    }
} // end of namespace