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
        wallRun = 64,
        hover = 128,
        jetpack = 256,
        phantom = 512,
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