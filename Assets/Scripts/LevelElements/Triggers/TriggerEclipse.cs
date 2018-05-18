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

            Debug.Log("I am " + (inverted ? "" : "not") + " inverted and the result is : " + !(args.EclipseOn ^ inverted));
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

        public override void Initialize(IGameController gameController)
        {
            base.Initialize(gameController);
            //Debug.Log ("INITIALISE" + transform.name);
            Game.Utilities.EventManager.EclipseEvent += OnEclipseEventHandler;
        }

        #endregion public methods

        //###########################################################
    }
} //end of namespace