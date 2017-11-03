﻿using System.Collections;
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
            public Player.UI.eUiState NewUiState { get; private set; }
            public Player.UI.eUiState PreviousUiState { get; private set; }

            public OnMenuSwitchedEventArgs(Player.UI.eUiState newUiState, Player.UI.eUiState previousUiState)
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
            public Player.UI.eUiState Menu { get; private set; }

            public OnShowMenuEventArgs(Player.UI.eUiState menu)
            {
                this.Menu = menu;
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

        #endregion model events

        //***********************************************************
        //***********************************************************

        #region gameplay events

        //###########################################################

        #region player spawned event

        public class OnPlayerSpawnedEventArgs : EventArgs
        {
            public Vector3 Position { get; private set; }

            public OnPlayerSpawnedEventArgs(Vector3 position)
            {
                this.Position = position;
            }
        }

        public delegate void OnPlayerSpawnedEventHandler(object sender, OnPlayerSpawnedEventArgs args);

        public static event OnPlayerSpawnedEventHandler OnPlayerSpawnedEvent;

        public static void SendOnPlayerSpawnedEvent(object sender, OnPlayerSpawnedEventArgs args)
        {
            OnPlayerSpawnedEvent?.Invoke(sender, args);
        }

        #endregion player spawned event

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

        #endregion gameplay events

        //***********************************************************
    }
} //end of namespace