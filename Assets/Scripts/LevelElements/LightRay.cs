using UnityEngine;

namespace Game.LevelElements
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(LineRenderer))]
    public class LightRay : MonoBehaviour
    {

        [SerializeField]
        Transform lookAtTarget;

        [SerializeField] bool inverseState;

        [SerializeField] Transform endOfRay;

        LightReceptor receptor;
        new LineRenderer renderer;
        Transform my;

        private void Start()
        {
            my = transform;
            renderer = GetComponent<LineRenderer>();
            renderer.useWorldSpace = false;
            renderer.positionCount = 2;
            renderer.SetPosition(0, Vector3.zero);
        }

        private void Update()
        {

            RaycastHit hit;
            if (Physics.Raycast(my.position, my.forward, out hit, Mathf.Infinity))
            {

                renderer.SetPosition(1, my.InverseTransformPoint(hit.point));
                LightReceptor newReceptor = hit.transform.GetComponent<LightReceptor>();

                if (newReceptor && newReceptor != receptor)
                {
                    if (receptor)
                    {
                        receptor.SetToggle(!inverseState, inverseState);
                    }

                    receptor = newReceptor;
                    receptor.SetToggle(inverseState, inverseState);
                }
                else if (receptor)
                {
                    //receptor.SetToggle(!inverseState, inverseState);
                }

                if (endOfRay)
                    endOfRay.position = hit.point;
            }

            if (lookAtTarget)
            {
                transform.LookAt(lookAtTarget);
                lookAtTarget.LookAt(my);
            }
        }
    }
} //end of namespace