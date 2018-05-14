using UnityEngine;
using System.Collections;
using Game.GameControl;

namespace Game.LevelElements
{
    public class TriggerEclipse : Trigger
    {

        //###########################################################

        #region private methods

        void OnEclipseEventHandler(object sender, Game.Utilities.EventManager.EclipseEventArgs args)
        {
            
            if (args.EclipseOn)
                SetTriggerState(false, true);
            else
                SetTriggerState(true, true);

        }

        #endregion private methods

        //###########################################################

        #region monobehaviour methods

        private void OnDisable()
        {
            Game.Utilities.EventManager.EclipseEvent -= OnEclipseEventHandler;
        }
        
        #endregion monobehaviour methods

        //###########################################################

        #region public methods

        public override void Initialize(IGameControllerBase gameController)
        {
            base.Initialize(gameController);
            //Debug.Log ("INITIALISE" + transform.name);
            Game.Utilities.EventManager.EclipseEvent += OnEclipseEventHandler;
        }

        #endregion public methods

        //###########################################################
    }
} //end of namespace