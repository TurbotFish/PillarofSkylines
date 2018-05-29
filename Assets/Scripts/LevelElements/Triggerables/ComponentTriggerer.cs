using System.Collections;
using System.Collections.Generic;
using Game.GameControl;
using UnityEngine;

namespace Game.LevelElements
{
    public class ComponentTriggerer : TriggerableObject
    {
        //###########################################################

        
        float elapsed;

        [Header("Component Triggerer")]
        public List<MonoBehaviour> componentWhenActivated;
        public List<MonoBehaviour> componentWhenDeactivated;

        //###########################################################

        #region public methods

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);
            

            if (Triggered)
            {
                if (componentWhenActivated.Count == 0 && componentWhenDeactivated.Count == 0)
                    return;


                for (int i = 0; i < componentWhenActivated.Count; i++)
                {
                    componentWhenActivated[i].enabled = true;
                }


                for (int i = 0; i < componentWhenDeactivated.Count; i++)
                {
                    componentWhenDeactivated[i].enabled = false;
                }
            }
            else
            {
                if (componentWhenActivated.Count == 0 && componentWhenDeactivated.Count == 0)
                    return;


                for (int i = 0; i < componentWhenActivated.Count; i++)
                {
                    componentWhenActivated[i].enabled = false;
                }


                for (int i = 0; i < componentWhenDeactivated.Count; i++)
                {
                    componentWhenDeactivated[i].enabled = true;
                }
            }
        }

        #endregion public methods

        //###########################################################

        #region protected methods

        protected override void Activate()
        {

            if (componentWhenActivated.Count == 0 && componentWhenDeactivated.Count == 0)
                return;


            for (int i = 0; i < componentWhenActivated.Count; i++)
            {
                componentWhenActivated[i].enabled = true;
            }


            for (int i = 0; i < componentWhenDeactivated.Count; i++)
            {
                componentWhenDeactivated[i].enabled = false;
            }
        }

        protected override void Deactivate()
        {
            if (componentWhenActivated.Count == 0 && componentWhenDeactivated.Count == 0)
                return;


            for (int i = 0; i < componentWhenActivated.Count; i++)
            {
                componentWhenActivated[i].enabled = false;
            }


            for (int i = 0; i < componentWhenDeactivated.Count; i++)
            {
                componentWhenDeactivated[i].enabled = true;
            }
        }

        #endregion protected methods

    }
} //end of namespace