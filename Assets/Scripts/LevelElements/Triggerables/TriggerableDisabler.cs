using Game.GameControl;
using UnityEngine;

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

        private Renderer rend;

        //###########################################################

        #region public methods

        public override void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            base.Initialize(gameController, isCopy);

            rend = GetComponent<Renderer>();

            for(int i = 0;i < objects.Length; i++)
            {
                var go = objects[i];
                if (go == null)
                {
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
            foreach (GameObject go in objects)
                go.SetActive(disabledByDefault);
        }

        protected override void Deactivate()
        {
            foreach (GameObject go in objects)
                go.SetActive(!disabledByDefault);
        }

        #endregion protected methods

        //###########################################################
    }
} //end of namespace