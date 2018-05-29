using System.Collections;
using Game.GameControl;
using UnityEngine;

namespace Game.LevelElements
{
    public class ChildTriggerer : TriggerableObject
    {
        //###########################################################

        [Header("Child Triggerer")]
        
        Transform my;
        float elapsed;

        //###########################################################

        #region public methods

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);

            my = transform;

            if (Triggered)
            {
                if (transform.childCount == 0)
                    return;


                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            else
            {
                if (transform.childCount == 0)
                    return;


                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        #endregion public methods

        //###########################################################

        #region protected methods

        protected override void Activate()
        {

            if (transform.childCount == 0)
                return;


            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        protected override void Deactivate()
        {
            if (transform.childCount == 0)
                return;


            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        #endregion protected methods
        
    }
} //end of namespace