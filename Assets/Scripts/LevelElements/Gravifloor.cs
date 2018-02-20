using Game.Player.CharacterController;
using System.Collections;
using UnityEngine;

public class Gravifloor : MonoBehaviour {
    
    Vector3 gravityDirection;
    static Gravifloor currentActive;

    float rotationDuration = 0.3f;

    float resetDelay = 0.1f; // just in case there is a microspace between two gravifloors
    [SerializeField] float resetDuration = 0.5f;
    
    Vector3 regularGravity = new Vector3(0, -1, 0);

    CharController currPlayer;


    private void Start()
    {
        gravityDirection = -transform.up;
        gameObject.tag = "Gravifloor";
    }

    public void AddPlayer(CharController player)
    {
        if (currentActive)
            currentActive.StopAllCoroutines();
        currentActive = this;
        currPlayer = player;

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

        for (float elapsed = 0; elapsed < rotationDuration; elapsed += Time.deltaTime)
        {
            player.ChangeGravityDirection(Vector3.Lerp(currentGravity, gravityGoal, elapsed / rotationDuration), player.MyTransform.position + player.MyTransform.up);
            yield return null;
        }
        player.ChangeGravityDirection(gravityGoal);
    }
    

    public IEnumerator _ResetGravity()
    {
        CharController player = currPlayer;

        yield return new WaitForSeconds(resetDelay);

        for (float elapsed = 0; elapsed < resetDuration; elapsed+=Time.deltaTime)
        {
            player.ChangeGravityDirection(Vector3.Lerp(gravityDirection, regularGravity, elapsed / resetDuration));
            yield return null;
        }
        player.ChangeGravityDirection(regularGravity);
    }

}
