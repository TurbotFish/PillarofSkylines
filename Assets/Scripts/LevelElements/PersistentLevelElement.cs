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

        // -- ATTRIBUTES

        public bool IsInitialized { get; private set; }

        protected GameController GameController { get; private set; }
        protected T PersistentData { get; private set; }

        //##################################################################

        // -- INITIALIZATION

        public virtual void Initialize(GameController gameController)
        {
            GameController = gameController;

            PersistentData = GameController.PlayerModel.GetPersistentDataObject(UniqueId) as T;

            if (PersistentData == null)
            {
                PersistentData = CreatePersistentDataObject() as T;
                gameController.PlayerModel.AddPersistentDataObject(PersistentData);
            }

            IsInitialized = true;
        }

        //##################################################################

        // -- INQUIRIES



        //##################################################################

        // -- OPERATIONS

        protected abstract PersistentData CreatePersistentDataObject();

        //##################################################################
    }
} // end of namespace