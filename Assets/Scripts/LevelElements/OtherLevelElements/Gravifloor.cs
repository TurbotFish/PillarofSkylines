using Game.Player.CharacterController;
using System.Collections;
using UnityEngine;

public class Gravifloor : MonoBehaviour {

    [SerializeField] bool customGavityDirection;

    [ConditionalHide("customGavityDirection")]
    public Vector3 gravityDirection;
    
    static Gravifloor currentActive;

    [SerializeField]
    float rotationSpeed = 180; // degree per second
    
    float resetDelay = 0.1f; // just in case there is a microspace between two gravifloors
    
    Vector3 regularGravity = new Vector3(0, -1, 0);

    CharController currPlayer;


    private void Start()
    {
        if (!customGavityDirection)
            gravityDirection = -transform.up;
        gameObject.tag = "Gravifloor";
    }

    private void OnValidate()
    {
        if (!customGavityDirection)
            gravityDirection = -transform.up;
    }

    public void AddPlayer(CharController player, Vector3 newGravity)
    {
        if (currentActive)
            currentActive.StopAllCoroutines();
        currentActive = this;
        currPlayer = player;
        StopAllCoroutines();
        StartCoroutine(_ChangeGravity(gravityDirection));
    }
    
    public void RemovePlayer(bool isJumping)
    {
        if (currentActive == this)
        {
            currPlayer.AddExternalVelocity(-gravityDirection*10, false, false);
            
            if (isJumping)
                StartCoroutine(_ResetGravity());
            else
                StartCoroutine(_ChangeGravity(regularGravity));
        }
    }
    
    public IEnumerator _ChangeGravity(Vector3 gravityGoal)
    {
        CharController player = currPlayer;
        
        Vector3 currentGravity = -player.MyTransform.up;

        float angle = Vector3.Angle(currentGravity, gravityGoal);
        float rotationDuration = angle / rotationSpeed;

        for (float elapsed = 0; elapsed < rotationDuration; elapsed += Time.deltaTime)
        {
            player.ChangeGravityDirection(Vector3.Slerp(currentGravity, gravityGoal, elapsed / rotationDuration), player.MyTransform.position + player.MyTransform.up);
            yield return null;
        }
        player.ChangeGravityDirection(gravityGoal);
    }
    

    public IEnumerator _ResetGravity()
    {
        CharController player = currPlayer;

        yield return new WaitForSeconds(resetDelay);

        float angle = Vector3.Angle(gravityDirection, regularGravity);
        float rotationDuration = angle / rotationSpeed;

        for (float elapsed = 0; elapsed < rotationDuration; elapsed+=Time.deltaTime)
        {
            player.ChangeGravityDirection(Vector3.Slerp(gravityDirection, regularGravity, elapsed / rotationDuration));
            yield return null;
        }
        player.ChangeGravityDirection(regularGravity);
    }

}
