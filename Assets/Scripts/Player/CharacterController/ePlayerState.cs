using System;

namespace Game.Player.CharacterController
{
    [Flags]
    public enum ePlayerState
    {
        air = 0, //jump & fall
        move = 1,
        slide = 2,
        stand = 4,
        glide = 8,
        dash = 16,
        windTunnel = 32,
        wallDrift = 64,
        wallRun = 128,
        wallClimb = 256,
        hover = 512,
        graviswap = 1024

        //exterminate!
        //inAir,
        //onGround,
        //gliding,
        //dashing,
        //sliding,
        //inWindTunnel,
        //WallDrifting,
        //WallRunningHorizontal,
        //WallRunningVertical
    }
} //end of namespace