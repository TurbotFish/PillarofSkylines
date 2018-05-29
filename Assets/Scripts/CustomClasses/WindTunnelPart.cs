using Game.GameControl;
using Game.LevelElements;
using Game.Player.CharacterController;
using Game.Player.CharacterController.States;
using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTunnelPart : MonoBehaviour, IInteractable, IWorldObject
{
    //########################################################################

    private static List<WindTunnelPart> activeWindTunnelParts = new List<WindTunnelPart>();

    [SerializeField] public float windStrength;
    [SerializeField] public float tunnelAttraction;
    [SerializeField] public int idInTunnel;

    private GameController gameController;
    private Transform myTransform;

    //########################################################################

    public void Initialize(GameController gameController)
    {
        this.gameController = gameController;
        myTransform = transform;
    }

    private void OnDestroy()
    {
        OnPlayerExit();
    }

    //########################################################################

    public Transform Transform { get { return transform; } }

    public Transform MyTransform
    {
        get
        {
            if (myTransform == null) { myTransform = transform; }
            return myTransform;
        }
    }

    public static bool IsPlayerInWindTunnel { get { return activeWindTunnelParts.Count != 0; } }

    public static List<WindTunnelPart> ActiveWindTunnelParts { get { return new List<WindTunnelPart>(activeWindTunnelParts); } }

    public bool IsInteractable()
    {
        return false;
    }

    //########################################################################

    public void OnPlayerEnter()
    {
        if (!activeWindTunnelParts.Contains(this))
        {
            activeWindTunnelParts.Add(this);

            if (gameController.PlayerController.CharController.stateMachine.CurrentState != ePlayerState.windTunnel)
            {
                //TODO: THIS SHOULD NOT BE HERE!
                var windTunnelState = new WindTunnelState(gameController.PlayerController.CharController, gameController.PlayerController.CharController.stateMachine);
                gameController.PlayerController.CharController.stateMachine.ChangeState(windTunnelState);
            }
        }
    }

    public void OnPlayerExit()
    {
        if (activeWindTunnelParts.Contains(this))
        {
            activeWindTunnelParts.Remove(this);
        }
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



    //private void Update()
    //{
    //    if (currentPlayer != null)
    //    {
    //        //Debug.Log("wind velocity = " + (transform.up * windStrength + Vector3.ProjectOnPlane(transform.position - currentPlayer.transform.position, transform.up) * tunnelAttraction) + " made from " + Vector3.ProjectOnPlane(transform.position - currentPlayer.transform.position, transform.up));
    //        //currentPlayer.AddWindVelocity(transform.up*windStrength + Vector3.ProjectOnPlane(transform.position - currentPlayer.transform.position, transform.up)*tunnelAttraction);
    //    }
    //}

    //public void AddPlayer(Game.Player.CharacterController.CharController player)
    //{
    //    currentPlayer = player;
    //}

    //public void RemovePlayer()
    //{
    //    //currentPlayer.ExitWindTunnel();
    //    currentPlayer = null;
    //}

    //########################################################################
}