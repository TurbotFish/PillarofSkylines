using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Utilities
{
    public static class EventManager
    {
        //***********************************************************

        #region ui events

        //###########################################################

        #region menu opened event

        public class OnMenuSwitchedEventArgs : EventArgs
        {
            public UI.eUiState NewUiState { get; private set; }
            public UI.eUiState PreviousUiState { get; private set; }

            public OnMenuSwitchedEventArgs(UI.eUiState newUiState, UI.eUiState previousUiState)
            {
                this.NewUiState = newUiState;
                this.PreviousUiState = previousUiState;
            }
        }

        public delegate void OnMenuSwitchedEventHandler(object sender, OnMenuSwitchedEventArgs args);

        public static event OnMenuSwitchedEventHandler OnMenuSwitchedEvent;

        public static void SendOnMenuSwitchedEvent(object sender, OnMenuSwitchedEventArgs args)
        {
            OnMenuSwitchedEvent?.Invoke(sender, args);
        }

        #endregion menu opened event

        //###########################################################
        //###########################################################

        #region show hud message event

        public class OnShowHudMessageEventArgs : EventArgs
        {
            public bool Show { get; private set; }
            public string Message { get; private set; }

            public OnShowHudMessageEventArgs(bool show, string message = null)
            {
                this.Show = show;
                this.Message = message;
            }
        }

        public delegate void OnShowHudMessageEventHandler(object sender, OnShowHudMessageEventArgs args);

        public static event OnShowHudMessageEventHandler OnShowHudMessageEvent;

        public static void SendShowHudMessageEvent(object sender, OnShowHudMessageEventArgs args)
        {
            OnShowHudMessageEvent?.Invoke(sender, args);
        }

        #endregion show hud message event

        //###########################################################
        //###########################################################

        #region show menu event

        public class OnShowMenuEventArgs : EventArgs
        {
            /// <summary>
            /// The id of the menu to switch to.
            /// </summary>
            public UI.eUiState Menu { get; private set; }

            /// <summary>
            /// Default Contructor.
            /// </summary>
            public OnShowMenuEventArgs(UI.eUiState menu)
            {
                this.Menu = menu;
            }
        }

        public class OnShowPillarEntranceMenuEventArgs : OnShowMenuEventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            public World.ePillarId PillarId { get; private set; }

            /// <summary>
            /// Constructor for the Pillar entrance menu.
            /// </summary>
            public OnShowPillarEntranceMenuEventArgs(World.ePillarId pillarId) : base(UI.eUiState.PillarEntrance)
            {
                this.PillarId = pillarId;
            }
        }

        public delegate void OnShowMenuEventHandler(object sender, OnShowMenuEventArgs args);

        public static event OnShowMenuEventHandler OnShowMenuEvent;

        public static void SendShowMenuEvent(object sender, OnShowMenuEventArgs args)
        {
            OnShowMenuEvent?.Invoke(sender, args);
        }

        #endregion show menu event

        //###########################################################

        #endregion ui events

        //***********************************************************
        //***********************************************************

        #region model events

        //###########################################################

        #region favour amount changed event

        public class OnFavourAmountChangedEventArgs : EventArgs
        {
            public int FavourAmount { get; private set; }

            public OnFavourAmountChangedEventArgs(int favourAmount)
            {
                this.FavourAmount = favourAmount;
            }
        }

        public delegate void OnFavourAmountChangedEventHandler(object sender, OnFavourAmountChangedEventArgs args);

        public static event OnFavourAmountChangedEventHandler OnFavourAmountChangedEvent;

        public static void SendOnFavourAmountChangedEvent(object sender, OnFavourAmountChangedEventArgs args)
        {
            OnFavourAmountChangedEvent?.Invoke(sender, args);
        }

        #endregion favour amount changed event

        //###########################################################
        //###########################################################

        #region pillar destroyed event

        public class PillarDestroyedEventArgs : EventArgs
        {
            public World.ePillarId PillarId { get; private set; }

            public PillarDestroyedEventArgs(World.ePillarId pillarId)
            {
                this.PillarId = pillarId;
            }
        }

        public delegate void PillarDestroyedEventHandler(object sender, PillarDestroyedEventArgs args);

        public static event PillarDestroyedEventHandler PillarDestroyedEvent;

        public static void SendPillarDestroyedEvent(object sender, PillarDestroyedEventArgs args)
        {
            PillarDestroyedEvent?.Invoke(sender, args);
        }

        #endregion pillar destroyed event

        //###########################################################

        #endregion model events

        //***********************************************************
        //***********************************************************

        #region gameplay events

        //###########################################################

        #region teleport player event

        public class OnTeleportPlayerEventArgs : EventArgs
        {
            /// <summary>
            /// The Position the Player should be teleported to.
            /// </summary>
            public Vector3 Position { get; private set; }

            /// <summary>
            /// value='true' means that the current scene was switched. value='false' means that the player is teleported inside the current scene.
            /// </summary>
            public bool IsNewScene { get; private set; }

            public OnTeleportPlayerEventArgs(Vector3 position, bool isNewScene)
            {
                this.Position = position;
            }
        }

        public delegate void OnTeleportPlayerEventHandler(object sender, OnTeleportPlayerEventArgs args);

        public static event OnTeleportPlayerEventHandler TeleportPlayerEvent;

        public static void SendTeleportPlayerEvent(object sender, OnTeleportPlayerEventArgs args)
        {
            TeleportPlayerEvent?.Invoke(sender, args);
        }

        #endregion teleport player event

        //###########################################################
        //###########################################################

        #region scene changed event

        public class OnSceneChangedEventArgs : EventArgs
        {
            public bool HasChangedToPillar { get; private set; }
            public World.ePillarId PillarId { get; private set; }

            /// <summary>
            /// Change to Open World.
            /// </summary>
            public OnSceneChangedEventArgs()
            {
                this.HasChangedToPillar = false;
            }

            /// <summary>
            /// Change to Pillar.
            /// </summary>
            /// <param name="pillarId"></param>
            public OnSceneChangedEventArgs(World.ePillarId pillarId)
            {
                this.HasChangedToPillar = true;
                this.PillarId = pillarId;
            }
        }

        public delegate void OnSceneChangedEventHandler(object sender, OnSceneChangedEventArgs args);

        public static event OnSceneChangedEventHandler OnSceneChangedEvent;

        public static void SendOnSceneChangedEvent(object sender, OnSceneChangedEventArgs args)
        {
            OnSceneChangedEvent?.Invoke(sender, args);
        }

        #endregion scene changed event

        //###########################################################
        //###########################################################

        #region enter pillar event

        public class OnEnterPillarEventArgs : EventArgs
        {
            public World.ePillarId PillarId { get; private set; }

            public OnEnterPillarEventArgs(World.ePillarId pillarId)
            {
                this.PillarId = pillarId;
            }
        }

        public delegate void OnEnterPillarEventHandler(object sender, OnEnterPillarEventArgs args);

        public static event OnEnterPillarEventHandler OnEnterPillarEvent;

        public static void SendOnEnterPillarEvent(object sender, OnEnterPillarEventArgs args)
        {
            OnEnterPillarEvent?.Invoke(sender, args);
        }

        #endregion enter pillar event

        //###########################################################
        //###########################################################

        #region eclipse events

        public class OnEclipseEventArgs : EventArgs
        {
            public bool EclipseOn;

            public OnEclipseEventArgs(bool eclipseOn)
            {
                EclipseOn = eclipseOn;
            }
        }

        public delegate void OnEclipseEventHandler(object sender, OnEclipseEventArgs args);

        public static event OnEclipseEventHandler OnEclipseEvent;

        public static void SendOnEclipseEvent(object sender, OnEclipseEventArgs args)
        {
            OnEclipseEvent?.Invoke(sender, args);
        }

        #endregion eclipse events


        //###########################################################
        //###########################################################

        #region eye killed events

        public delegate void OnEyeKilledEventHandler(object sender);

        public static event OnEyeKilledEventHandler OnEyeKilledEvent;

        public static void SendOnEyeKilledEvent(object sender)
        {
            OnEyeKilledEvent?.Invoke(sender);
        }

        #endregion eye killed events

        //###########################################################
        //###########################################################

        #region favour picked up event

        public class FavourPickedUpEventArgs : EventArgs
        {
            public int FavourId { get; private set; }

            public FavourPickedUpEventArgs(int favourId)
            {
                this.FavourId = favourId;
            }
        }

        public delegate void FavourPickedUpEventHandler(object sender, FavourPickedUpEventArgs args);

        public static event FavourPickedUpEventHandler FavourPickedUpEvent;

        public static void SendFavourPickedUpEvent(object sender, FavourPickedUpEventArgs args)
        {
            FavourPickedUpEvent?.Invoke(sender, args);
        }

        #endregion favour picked up event

        //###########################################################

        #endregion gameplay events

        //***********************************************************
    }
} //end of namespace