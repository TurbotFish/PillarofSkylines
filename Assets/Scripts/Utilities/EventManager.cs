﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Model;

namespace Game.Utilities
{
    public static class EventManager
    {
        //###########################################################
        #region ui events
        //###########################################################
        //***********************************************************
        #region menu opened event
        //***********************************************************

        public class OnMenuSwitchedEventArgs : EventArgs
        {
            public UI.MenuType NewUiState { get; private set; }
            public UI.MenuType PreviousUiState { get; private set; }

            public OnMenuSwitchedEventArgs(UI.MenuType newUiState, UI.MenuType previousUiState)
            {
                NewUiState = newUiState;
                PreviousUiState = previousUiState;
            }
        }

        public delegate void OnMenuSwitchedEventHandler(object sender, OnMenuSwitchedEventArgs args);

        public static event OnMenuSwitchedEventHandler OnMenuSwitchedEvent;

        public static void SendOnMenuSwitchedEvent(object sender, OnMenuSwitchedEventArgs args)
        {
            OnMenuSwitchedEvent?.Invoke(sender, args);
        }

        //***********************************************************
        #endregion menu opened event
        //***********************************************************
        //***********************************************************
        #region show hud message event
        //***********************************************************

        public class OnShowHudMessageEventArgs : EventArgs
        {
            public bool Show { get; private set; }
            public string Message { get; private set; }
            public string Description { get; private set; }
            public float Time { get; private set; }
            public UI.eMessageType MessageType { get; private set; }

            public OnShowHudMessageEventArgs(bool show, string message = null, UI.eMessageType messageType = 0, string description = "", float time = 2)
            {
                Show = show;
                Message = message;
                MessageType = messageType;
                Description = description;
                Time = time;
            }
        }

        public delegate void OnShowHudMessageEventHandler(object sender, OnShowHudMessageEventArgs args);

        public static event OnShowHudMessageEventHandler OnShowHudMessageEvent;

        public static void SendShowHudMessageEvent(object sender, OnShowHudMessageEventArgs args)
        {
            OnShowHudMessageEvent?.Invoke(sender, args);
        }

        //***********************************************************
        #endregion show hud message event
        //***********************************************************
        //***********************************************************
        #region show menu event
        //***********************************************************

        public class OnShowMenuEventArgs : EventArgs
        {
            /// <summary>
            /// The id of the menu to switch to.
            /// </summary>
            public UI.MenuType Menu { get; private set; }

            /// <summary>
            /// Default Contructor.
            /// </summary>
            public OnShowMenuEventArgs(UI.MenuType menu)
            {
                Menu = menu;
            }
        }

        public class OnShowPillarEntranceMenuEventArgs : OnShowMenuEventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            public PillarId PillarId { get; private set; }

            /// <summary>
            /// Constructor for the Pillar entrance menu.
            /// </summary>
            public OnShowPillarEntranceMenuEventArgs(PillarId pillarId) : base(UI.MenuType.PillarEntrance)
            {
                PillarId = pillarId;
            }
        }

        public class OnShowLoadingScreenEventArgs : OnShowMenuEventArgs
        {
            public int Id { get; private set; }

            public OnShowLoadingScreenEventArgs() : base(UI.MenuType.LoadingScreen)
            {
                Id = -1;
            }

            public OnShowLoadingScreenEventArgs(PillarId pillarId) : base(UI.MenuType.LoadingScreen)
            {
                Id = (int)pillarId;
            }
        }

        public delegate void OnShowMenuEventHandler(object sender, OnShowMenuEventArgs args);

        public static event OnShowMenuEventHandler OnShowMenuEvent;

        public static void SendShowMenuEvent(object sender, OnShowMenuEventArgs args)
        {
            OnShowMenuEvent?.Invoke(sender, args);
        }

        //***********************************************************
        #endregion show menu event
        //***********************************************************
        //###########################################################
        #endregion ui events
        //###########################################################


        //###########################################################
        #region model events
        //###########################################################
        //***********************************************************
        #region pillar state changed event
        //***********************************************************

        public class PillarStateChangedEventArgs : EventArgs
        {
            public PillarId PillarId { get; private set; }
            public PillarState PillarState { get; private set; }

            public PillarStateChangedEventArgs(PillarId pillar_id, PillarState pillar_state)
            {
                PillarId = pillar_id;
                PillarState = pillar_state;
            }
        }

        public delegate void PillarStateChangedEventHandler(object sender, PillarStateChangedEventArgs args);
        public static event PillarStateChangedEventHandler PillarStateChangedEvent;

        public static void SendPillarStateChangedEvent(object sender, PillarStateChangedEventArgs args)
        {
            PillarStateChangedEvent?.Invoke(sender, args);
        }

        //***********************************************************
        #endregion pillar state changed event
        //***********************************************************
        //***********************************************************
        #region ability state changed
        //***********************************************************

        public class AbilityStateChangedEventArgs : EventArgs
        {
            public AbilityType AbilityType { get; private set; }
            public AbilityState AbilityState { get; private set; }

            public AbilityStateChangedEventArgs(AbilityType ability_type, AbilityState ability_state)
            {
                AbilityType = ability_type;
                AbilityState = ability_state;
            }
        }

        public delegate void AbilityStateChangedEventHandler(object sender, AbilityStateChangedEventArgs args);
        public static event AbilityStateChangedEventHandler AbilityStateChangedEvent;

        public static void SendAbilityStateChangedEvent(object sender, AbilityStateChangedEventArgs args)
        {
            AbilityStateChangedEvent?.Invoke(sender, args);
        }

        //***********************************************************
        #endregion ability state changed
        //***********************************************************
        //***********************************************************
        #region pillar mark state changed
        //***********************************************************

        public class PillarMarkStateChangedEventArgs : EventArgs
        {
            public PillarMarkId PillarMarkId { get; private set; }
            public PillarMarkState PillarMarkState { get; private set; }

            public PillarMarkStateChangedEventArgs(PillarMarkId pillar_mark_id, PillarMarkState pillar_mark_state)
            {
                PillarMarkId = pillar_mark_id;
                PillarMarkState = pillar_mark_state;
            }
        }

        public delegate void PillarMarkStateChangedEventHandler(object sender, PillarMarkStateChangedEventArgs args);
        public static event PillarMarkStateChangedEventHandler PillarMarkStateChangedEvent;

        public static void SendPillarMarkStateChangedEvent(object sender, PillarMarkStateChangedEventArgs args)
        {
            PillarMarkStateChangedEvent?.Invoke(sender, args);
        }

        //***********************************************************
        #endregion pillar mark state changed
        //***********************************************************
        //###########################################################
        #endregion model events
        //###########################################################

        //###########################################################
        #region gameplay events
        //###########################################################
        //***********************************************************

        #region game paused event

        public class GamePausedEventArgs : EventArgs
        {
            public bool PauseActive { get; private set; }

            public GamePausedEventArgs(bool pauseActive)
            {
                PauseActive = pauseActive;
            }
        }

        public delegate void GamePausedEventHandler(object sender, GamePausedEventArgs args);

        public static event GamePausedEventHandler GamePausedEvent;

        public static void SendGamePausedEvent(object sender, GamePausedEventArgs args)
        {
            GamePausedEvent?.Invoke(sender, args);
        }

        #endregion game paused event

        //***********************************************************

        #region teleport player event

        public class TeleportPlayerEventArgs : EventArgs
        {
            /// <summary>
            /// The Position the Player should be teleported from.
            /// </summary>
            public Vector3 FromPosition { get; private set; }

            /// <summary>
            /// The Position the Player should be teleported to.
            /// </summary>
            public Vector3 Position { get; private set; }

            /// <summary>
            /// The Rotation the Player should have after teleportation (only on New Scene)
            /// </summary>
            public Quaternion Rotation { get; private set; }

            /// <summary>
            /// value='true' means that the current scene was switched. value='false' means that the player is teleported inside the current scene.
            /// </summary>
            public bool IsNewScene { get; private set; }

            /// <summary>
            /// Whether or not the player should change its rotation on teleport.
            /// </summary>
            public bool TakeRotation { get; private set; }

            public TeleportPlayerEventArgs(Vector3 position, bool isNewScene)
            {
                Position = position;
                Rotation = Quaternion.identity;
                IsNewScene = isNewScene;
                TakeRotation = false;
            }

            public TeleportPlayerEventArgs(Vector3 position, Quaternion rotation, bool isNewScene)
            {
                Position = position;
                Rotation = rotation;
                IsNewScene = isNewScene;
                TakeRotation = true;
            }

            public TeleportPlayerEventArgs(Vector3 fromPosition, Vector3 position, bool isNewScene)
            {
                FromPosition = fromPosition;
                Position = position;
                Rotation = Quaternion.identity;
                IsNewScene = isNewScene;
                TakeRotation = false;
            }

            public TeleportPlayerEventArgs(Vector3 fromPosition, Vector3 position, Quaternion rotation, bool isNewScene)
            {
                FromPosition = fromPosition;
                Position = position;
                Rotation = rotation;
                IsNewScene = isNewScene;
                TakeRotation = true;
            }
        }

        public delegate void TeleportPlayerEventHandler(object sender, TeleportPlayerEventArgs args);

        public static event TeleportPlayerEventHandler TeleportPlayerEvent;

        public static void SendTeleportPlayerEvent(object sender, TeleportPlayerEventArgs args)
        {
            TeleportPlayerEvent?.Invoke(sender, args);
        }

        #endregion teleport player event

        //***********************************************************

        #region scene change events

        public class PreSceneChangeEventArgs : EventArgs
        {
            public bool ChangingToOpenWorld { get; private set; }
            public bool ChangingToPillar { get; private set; }

            public PreSceneChangeEventArgs(bool changingToOpenWorld)
            {
                ChangingToOpenWorld = changingToOpenWorld;
                ChangingToPillar = !changingToOpenWorld;
            }
        }

        public delegate void PreSceneChangeEventHandler(object sender, PreSceneChangeEventArgs args);

        public static event PreSceneChangeEventHandler PreSceneChangeEvent;

        public static void SendPreSceneChangeEvent(object sender, PreSceneChangeEventArgs args)
        {
            PreSceneChangeEvent?.Invoke(sender, args);
        }

        public class SceneChangedEventArgs : EventArgs
        {
            public bool HasChangedToPillar { get; private set; }
            public PillarId PillarId { get; private set; }

            /// <summary>
            /// Change to Open World.
            /// </summary>
            public SceneChangedEventArgs()
            {
                this.HasChangedToPillar = false;
            }

            /// <summary>
            /// Change to Pillar.
            /// </summary>
            /// <param name="pillarId"></param>
            public SceneChangedEventArgs(PillarId pillarId)
            {
                this.HasChangedToPillar = true;
                this.PillarId = pillarId;
            }
        }

        public delegate void SceneChangedEventHandler(object sender, SceneChangedEventArgs args);

        public static event SceneChangedEventHandler SceneChangedEvent;

        public static void SendSceneChangedEvent(object sender, SceneChangedEventArgs args)
        {
            SceneChangedEvent?.Invoke(sender, args);
            //Debug.Log("Event sent: SceneChanged");
        }

        #endregion scene change events

        //***********************************************************

        #region eclipse event

        public class EclipseEventArgs : EventArgs
        {
            public bool EclipseOn;

            public EclipseEventArgs(bool eclipseOn)
            {
                EclipseOn = eclipseOn;
            }
        }

        public delegate void EclipseEventHandler(object sender, EclipseEventArgs args);

        public static event EclipseEventHandler EclipseEvent;

        public static void SendEclipseEvent(object sender, EclipseEventArgs args)
        {
            EclipseEvent?.Invoke(sender, args);
        }

        #endregion eclipse event

        //***********************************************************

        #region pick-up collected event

        public class PickupCollectedEventArgs : EventArgs
        {
            public string PickupID { get; private set; }

            public PickupCollectedEventArgs(string favourId)
            {
                PickupID = favourId;
            }
        }

        public delegate void PickupCollectedEventHandler(object sender, PickupCollectedEventArgs args);

        public static event PickupCollectedEventHandler PickupCollectedEvent;

        public static void SendPickupCollectedEvent(object sender, PickupCollectedEventArgs args)
        {
            PickupCollectedEvent?.Invoke(sender, args);
        }

        #endregion pick-up collected event

        //***********************************************************

        #region echo destroyed event

        public delegate void EchoDestroyedEventHandler(object sender);

        public static event EchoDestroyedEventHandler EchoDestroyedEvent;

        public static void SendEchoDestroyedEvent(object sender)
        {
            EchoDestroyedEvent?.Invoke(sender);
        }

        #endregion echo destroyed event

        //***********************************************************

        #region trigger updated

        public class TriggerUpdatedEventArgs : EventArgs
        {
            public string TriggerId { get; private set; }
            public LevelElements.Trigger Trigger { get; private set; }

            public TriggerUpdatedEventArgs(LevelElements.Trigger trigger)
            {
                TriggerId = trigger.UniqueId;
                Trigger = trigger;
            }
        }

        public delegate void TriggerUpdatedEventHandler(object sender, TriggerUpdatedEventArgs args);

        public static event TriggerUpdatedEventHandler TriggerUpdatedEvent;

        public static void SendTriggerUpdatedEvent(object sender, TriggerUpdatedEventArgs args)
        {
            TriggerUpdatedEvent?.Invoke(sender, args);
        }

        #endregion trigger updated

        //***********************************************************

        #endregion gameplay events

        //###########################################################
    }
} //end of namespace