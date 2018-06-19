using UnityEngine;
using System.Collections;
using Game.GameControl;

namespace Game.LevelElements
{
    public class TriggerEclipse : Trigger
    {

        public bool inverted;

        //###########################################################

        #region private methods

        void OnEclipseEventHandler(object sender, Game.Utilities.EventManager.EclipseEventArgs args)
        {

            SetTriggerState(!(args.EclipseOn ^ inverted), true);

            /*
            if (args.EclipseOn)
                SetTriggerState(false, true);
            else
                SetTriggerState(true, true);
                */
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

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);
            //Debug.Log ("INITIALISE" + transform.name);
            Game.Utilities.EventManager.EclipseEvent += OnEclipseEventHandler;
        }

        protected override void OnTriggerStateChanged(bool old_state, bool new_state)
        {
            //throw new System.NotImplementedException();
        }

        #endregion public methods

        //###########################################################
    }
} //end of namespace