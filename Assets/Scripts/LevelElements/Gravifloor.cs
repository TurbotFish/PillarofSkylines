
using Game.Player.CharacterController;
using UnityEngine;

public class Gravifloor : MovingPlatform {
    
    Vector3 gravityDirection, trueGravityDirection;

    private void Awake()
    {
        trueGravityDirection = Vector3.down;
        gravityDirection = -transform.up;
    }

    public override void AddPlayer(CharController player, Vector3 playerImpactPoint)
    {
        base.AddPlayer(player, playerImpactPoint);
        
        currPlayer.ChangeGravityDirection(gravityDirection);
    }

    public override void RemovePlayer()
    {
        base.RemovePlayer();

    }

}
