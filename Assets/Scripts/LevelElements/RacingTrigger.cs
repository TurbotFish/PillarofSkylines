using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class RacingTrigger : Trigger
{
    [SerializeField] Transform racer;
    [SerializeField] Transform target;

    [Space]

    [SerializeField] float timeToReachTarget = 5;
    [SerializeField] float timeActive = 1;
    [SerializeField] BezierSpline spline;
    
    bool racing;

    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnValidate()
    {
        if (!racer) racer = transform;
        if (!target && Targets[0]) target = Targets[0].transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target && !racing)
            StartCoroutine(Race());
    }

    IEnumerator Race()
    {
        Vector3 startPosition = racer.position;
        racing = true;
        
        for(float elapsed = 0; elapsed < timeToReachTarget; elapsed +=Time.deltaTime)
        {
            float t = elapsed / timeToReachTarget;
            if (spline)
            {
                racer.position = spline.GetPoint(t);
                print("POSITION " + t + " ON SPLINE " + spline.GetPoint(t));
            }
            else
                racer.position = Vector3.Lerp(startPosition, target.position, t);
            yield return null;
        }

        TriggerState = true;

        yield return new WaitForSeconds(timeActive);

        TriggerState = false;
        racer.position = startPosition;
        racing = false;
    }
}
