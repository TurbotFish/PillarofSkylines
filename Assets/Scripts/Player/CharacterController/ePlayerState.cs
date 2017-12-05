
namespace Game.Player.CharacterController
{
    public enum ePlayerState
    {
        empty,
        fall,
        jump,
        move,
        stand,
        glide,
        dash,
        windTunnel,
        wallDrift,
        wallRun,
        wallClimb,

        //exterminate
        inAir,
        onGround,
        gliding,
        dashing,
        sliding,
        inWindTunnel,
        WallDrifting,
        WallRunningHorizontal,
        WallRunningVertical
    }
} //end of namespace