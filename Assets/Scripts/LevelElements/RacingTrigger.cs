using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class RacingTrigger : Trigger
{
    [SerializeField] float timeToReachTarget = 5;
    [SerializeField] float timeActive = 1;

    Transform my, target;
    bool racing;

    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        my = transform;
        if (targets[0])
            target = targets[0].transform;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (target && !racing)
            StartCoroutine(Race());
    }

    IEnumerator Race()
    {
        Vector3 startPosition = my.position;
        racing = true;
        
        for(float elapsed = 0; elapsed < timeToReachTarget; elapsed +=Time.deltaTime)
        {
            float t = elapsed / timeToReachTarget;
            my.position = Vector3.Lerp(startPosition, target.position, t);
            yield return null;
        }

        TriggerState = true;

        yield return new WaitForSeconds(timeActive);

        TriggerState = false;
        my.position = startPosition;
        racing = false;
    }
    
}
