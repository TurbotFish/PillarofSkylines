using System.Collections;
using Game.GameControl;
using UnityEngine;

namespace Game.LevelElements
{
    public class Cog : TriggerableObject
    {
        //###########################################################
        
        [Header("Child Triggerer")]
        [SerializeField]
        private float rotationSpeed;
        [SerializeField]
        private axis rotationAxis;

        private Transform my;

        //###########################################################

        public Transform MyTransform { get { if (my == null) { my = transform; } return my; } }

        //###########################################################

        #region public methods

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);

            if (my == null)
            {
                my = transform;
            }
        }

        #endregion public methods

        //###########################################################

        #region protected methods

        protected override void Activate()
        {
        }

        protected override void Deactivate()
        {
        }

        #endregion protected methods

        //###########################################################

        #region private methods
        
        void Update()
        {
            if (!IsInitialized)
                return;

            if (Triggered)
            {
                switch (rotationAxis)
                {
                    case axis.XAxis:
                        my.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
                        break;
                    case axis.YAxis:
                        my.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
                        break;
                    case axis.ZAxis:
                        my.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion private methods

        //###########################################################
    }

    enum axis
    {
        XAxis,
        YAxis,
        ZAxis
    }
} //end of namespace