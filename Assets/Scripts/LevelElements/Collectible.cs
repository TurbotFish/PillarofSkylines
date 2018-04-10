using System.Collections;
using UnityEngine;

public class Collectible : Interactible {

    // speed
    float speed = 4f;
    float speedVariance = 3f;
    // dance
    float danceDuration = 0.8f;
    float danceAngle = 360 * 3;
    // pos
    float minRadius = 0.3f;
    float maxRadius = 0.7f;

    [SerializeField] ParticleSystem feedback;

    bool triggered;
    bool followingPlayer;
    Vector3 targetOffset, pos, randomAxis;
    Transform my, pilou;
    int timeSet = 0;

    private void Update()
    {
        if (followingPlayer) {
            my.position = Vector3.Lerp(my.position, TargetPoint(), speed * Time.deltaTime);
        }
        // il faudrait une idle
    }

    void StartFollowingPlayer()
    {
        pos = my.position;
        followingPlayer = true;
    }

    void UpdateTargetPoint() {
        targetOffset.x = Random.value > 0.5f ? Random.Range(-maxRadius, -minRadius) : Random.Range(minRadius, maxRadius);
        targetOffset.y = Random.Range(0.5f, 1.5f);
        targetOffset.z = Random.value > 0.5f ? Random.Range(-maxRadius, -minRadius) : Random.Range(minRadius, maxRadius);
    }

    void Dance() {
        StartCoroutine(_Dance());
    }
    
    public override void EnterTrigger(Transform player)
    {
        if (!triggered)
        {
            triggered = true;
            my = transform;
            pilou = player;

            Instantiate(feedback, my.position, my.rotation).Play();

            UpdateTargetPoint();
            speed += Random.Range(-speedVariance, speedVariance);

            Invoke("Dance", 0.4f);
        }

    }

    public override void ExitTrigger(Transform player)
    {
        //if (triggered && !followingPlayer)
        //    StartFollowingPlayer();
    }

    
    IEnumerator _Dance() {
        Vector3 dancePivot = pilou.position;
        dancePivot.y += targetOffset.y;

        float targetRadius = (dancePivot - TargetPoint()).magnitude;
        
        for(float elapsed = 0; elapsed < danceDuration || Vector3.SqrMagnitude(my.position - TargetPoint()) > 0.5f; elapsed += Time.deltaTime)
        {
            float t = elapsed / danceDuration;

            float radius = Mathf.Lerp(minRadius, targetRadius, t);

            dancePivot = pilou.position;
            dancePivot.y += targetOffset.y;

            my.RotateAround(dancePivot, pilou.up, danceAngle * Time.deltaTime / (elapsed*elapsed + danceDuration) );

            Vector3 pos = (my.position - dancePivot).normalized * radius + dancePivot;
            pos.y = dancePivot.y;

            my.position = Vector3.MoveTowards(my.position, pos, Time.deltaTime * speed);

            yield return null;
        }
        StartFollowingPlayer();
    }


    Vector3 TargetPoint()
    {
        return pilou.position + targetOffset;
        //return pilou.TransformPoint(targetOffset);
    }
}
