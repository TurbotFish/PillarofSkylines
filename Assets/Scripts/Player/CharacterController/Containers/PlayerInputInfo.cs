﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.Containers
{
    public struct PlayerInputInfo
    {
        public Vector3 leftStickRaw;
        public Vector3 leftStickToCamera;
        public Vector3 leftStickToSlope;
        public bool leftStickAtZero;

        public bool jumpButton;
        public bool jumpButtonDown;
        public bool jumpButtonUp;

        public bool dashButton;
        public bool dashButtonDown;
        public bool dashButtonUp;

        public bool sprintButton;
        public bool sprintButtonDown;
        public bool sprintButtonUp;

        public bool jetpackButton;
        public bool jetpackButtonDown;
        public bool jetpackButtonUp;

        public bool rightStickButtonDown;

        public void Reset()
        {
            leftStickRaw = Vector3.zero;
            leftStickToCamera = Vector3.zero;
            leftStickToSlope = Vector3.zero;
            leftStickAtZero = true;

            jumpButton = false;
            jumpButtonDown = false;
            jumpButtonUp = false;

            dashButton = false;
            dashButtonDown = false;
            dashButtonUp = false;

            sprintButton = false;
            sprintButtonDown = false;
            sprintButtonUp = false;

            jetpackButton = false;
            jetpackButtonDown = false;
            jetpackButtonUp = false;

            rightStickButtonDown = false;
        }
    }
}