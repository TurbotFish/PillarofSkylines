using Game.GameControl;
using Game.LevelElements;
using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyingpillar : MonoBehaviour, IInteractable, IWorldObject
{
    //##################################################################

    [SerializeField] public MovingPlatform movingPillar;
    [SerializeField] public float damp = 0.2f;

    bool here;
    Vector3 targetPos;
    Transform player, move;

    //##################################################################

    #region initialization

    public void Initialize(GameController gameController)
    {
        move = movingPillar.transform;
        targetPos = move.position;
        player = gameController.PlayerController.transform;
    }

    #endregion initialization

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

    public void OnPlayerEnter()
    {
        here = true;
    }

    public void OnPlayerExit()
    {
        here = false;
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
        if (here)
        {
            targetPos.y = player.position.y;
            move.position = Vector3.Lerp(move.position, targetPos, Time.deltaTime / damp);
        }
    }

    #endregion operations

    //##################################################################
}
