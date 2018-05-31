using Game.GameControl;
using Game.World;
using UnityEngine;

namespace Game.LevelElements
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(LineRenderer))]
    public class LightRay : MonoBehaviour, IWorldObject
    {
        //###########################################################

        [SerializeField]
        private Transform lookAtTarget;

        [SerializeField]
        private bool inverseState;

        [SerializeField]
        private Transform endOfRay;

        private LightReceptor receptor;
        private new LineRenderer renderer;
        private Transform my;
        private bool isInitialized = false;

        //###########################################################

        #region monobehaviour methods

        private void Update()
        {
            if (!isInitialized)
            {
                return;
            }

            RaycastHit hit;
            if (Physics.Raycast(my.position, my.forward, out hit, Mathf.Infinity))
            {
                renderer.SetPosition(1, my.InverseTransformPoint(hit.point));
                LightReceptor newReceptor = hit.transform.GetComponent<LightReceptor>();

                if (newReceptor && newReceptor != receptor) //the ray has hit a new receptor
                {
                    if (receptor) //the ray was previously hitting another receptor
                    {
                        receptor.SetToggle(!inverseState, inverseState); //deactivate(?) current receptor
                    }

                    receptor = newReceptor; //replace the current receptor
                    receptor.SetToggle(inverseState, inverseState); //activate the new receptor
                }
                else if (!newReceptor && receptor) //the ray is not hitting any receptor anymore
                {
                    receptor.SetToggle(!inverseState, inverseState); //deactivate(?) current receptor
                    receptor = null;
                }

                if (endOfRay)
                {
                    endOfRay.position = hit.point;
                }
            }

            if (lookAtTarget)
            {
                transform.LookAt(lookAtTarget);
                lookAtTarget.LookAt(my);
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        #region public methods

        public void Initialize(GameController gameController)
        {
            my = transform;
            renderer = GetComponent<LineRenderer>();
            renderer.useWorldSpace = false;
            renderer.positionCount = 2;
            renderer.SetPosition(0, Vector3.zero);

            isInitialized = true;
        }

        #endregion public methods

        //###########################################################
    }
} //end of namespace