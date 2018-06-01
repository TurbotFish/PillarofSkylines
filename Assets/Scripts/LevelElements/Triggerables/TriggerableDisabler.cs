using Game.GameControl;
using UnityEngine;
using System.Collections;

namespace Game.LevelElements
{
    public class TriggerableDisabler : TriggerableObject
    {
        //###########################################################

        [Header("Disabler")]
        [SerializeField]
        private bool disabledByDefault;

        [SerializeField]
        private GameObject[] objects;

        [SerializeField]
        float delayBeforeActivation = 0;

        private Renderer rend;

        //###########################################################

        #region public methods

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);

            rend = GetComponent<Renderer>();

            for(int i = 0;i < objects.Length; i++)
            {
                var go = objects[i];
                if (go == null) {
                    Debug.LogErrorFormat("TriggerableDisabler: Initialize: element {0} of objects array is null!", i);
                }
                else
                {
                    go.SetActive(!disabledByDefault);
                }
            }
        }

        #endregion public methods

        //###########################################################

        #region protected methods

        protected override void Activate()
        {
            StartCoroutine(_Activate(disabledByDefault));
        }

        protected override void Deactivate()
        {
            StartCoroutine(_Activate(!disabledByDefault));
        }

        IEnumerator _Activate(bool active)
        {
            yield return new WaitForSeconds(delayBeforeActivation);

            foreach (GameObject go in objects)
                go.SetActive(active);
        }


        #endregion protected methods

        //###########################################################
    }
} //end of namespace