using Game.GameControl;
using Game.LevelElements;
using Game.World;
using System.Collections;
using UnityEngine;

public class Collectible : MonoBehaviour, IInteractable, IWorldObject
{
    //##################################################################

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

    // state
    bool triggered;
    bool followingPlayer;
    public bool collected;
    // properties
    Vector3 targetPoint;
    Vector3 targetOffset;
    Vector3 pos;
    Vector3 randomAxis;
    Transform myTransform;
    Transform pilouTransform;
    int timeSet = 0;

    //##################################################################

    public void Initialize(IGameController gameController)
    {
        myTransform = transform;
        pilouTransform = gameController.PlayerController.transform;
    }

    //##################################################################

    #region inquiries

    public Transform Transform { get { return transform; } }

    public bool IsInteractable()
    {
        return false;
    }

    #endregion inquiries

    //##################################################################

    #region operations

    public void GoToCollector(Vector3 position)
    {
        followingPlayer = false;
        collected = true;
        targetPoint = position;
    }

    public void OnPlayerEnter()
    {
        if (!triggered)
        {
            triggered = true;

            Instantiate(feedback, myTransform.position, myTransform.rotation).Play();

            UpdateTargetPoint();
            speed += Random.Range(-speedVariance, speedVariance);

            Invoke("Dance", 0.4f);
        }
    }

    public void OnPlayerExit()
    {
        //if (triggered && !followingPlayer)
        //    StartFollowingPlayer();
    }

    public void OnHoverBegin()
    {
    }

    public void OnHoverEnd()
    {
    }

    public void OnInteraction()
    {
    }

    private void Update()
    {
        if (followingPlayer)
        {
            targetPoint = pilouTransform.position + targetOffset;
            myTransform.position = Vector3.Lerp(myTransform.position, targetPoint, speed * Time.deltaTime);
        }
        else if (collected)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, targetPoint, speed * Time.deltaTime);
        }
        // il faudrait une idle
    }

    private void StartFollowingPlayer()
    {
        pos = myTransform.position;
        followingPlayer = true;
    }

    private void UpdateTargetPoint()
    {
        targetOffset.x = Random.value > 0.5f ? Random.Range(-maxRadius, -minRadius) : Random.Range(minRadius, maxRadius);
        targetOffset.y = Random.Range(0.5f, 1.5f);
        targetOffset.z = Random.value > 0.5f ? Random.Range(-maxRadius, -minRadius) : Random.Range(minRadius, maxRadius);
    }

    private void Dance()
    {
        StartCoroutine(_Dance());
    }

    private IEnumerator _Dance()
    {
        Vector3 dancePivot = pilouTransform.position;
        dancePivot.y += targetOffset.y;

        targetPoint = pilouTransform.position + targetOffset;
        float targetRadius = (dancePivot - targetPoint).magnitude;

        for (float elapsed = 0; elapsed < danceDuration || Vector3.SqrMagnitude(myTransform.position - targetPoint) > 0.5f; elapsed += Time.deltaTime)
        {
            float t = elapsed / danceDuration;

            float radius = Mathf.Lerp(minRadius, targetRadius, t);

            dancePivot = pilouTransform.position;
            dancePivot.y += targetOffset.y;

            myTransform.RotateAround(dancePivot, pilouTransform.up, danceAngle * Time.deltaTime / (elapsed * elapsed + danceDuration));

            Vector3 pos = (myTransform.position - dancePivot).normalized * radius + dancePivot;
            pos.y = dancePivot.y;

            myTransform.position = Vector3.MoveTowards(myTransform.position, pos, Time.deltaTime * speed);

            targetPoint = pilouTransform.position + targetOffset;

            yield return null;
        }
        StartFollowingPlayer();
    }

    #endregion operations

    //##################################################################
}
