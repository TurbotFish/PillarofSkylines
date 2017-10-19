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

        public class OnMenuOpenedEventArgs : EventArgs
        {
            public Player.UI.eUiState Menu { get; private set; }

            public OnMenuOpenedEventArgs(Player.UI.eUiState menu)
            {
                this.Menu = menu;
            }
        }

        public delegate void OnMenuOpenedEventHandler(object sender, OnMenuOpenedEventArgs args);

        public static event OnMenuOpenedEventHandler OnMenuOpenedEvent;

        public static void SendOnMenuOpenedEvent(object sender, OnMenuOpenedEventArgs args)
        {
            OnMenuOpenedEvent?.Invoke(sender, args);
        }

        #endregion menu opened event

        //###########################################################
        //###########################################################

        #region menu closed event

        public class OnMenuClosedEventArgs : EventArgs
        {
            public Player.UI.eUiState Menu { get; private set; }

            public OnMenuClosedEventArgs(Player.UI.eUiState menu)
            {
                this.Menu = menu;
            }
        }

        public delegate void OnMenuClosedEventHandler(object sender, OnMenuClosedEventArgs args);

        public static event OnMenuClosedEventHandler OnMenuClosedEvent;

        public static void SendOnMenuClosedEvent(object sender, OnMenuClosedEventArgs args)
        {
            OnMenuClosedEvent?.Invoke(sender, args);
        }

        #endregion menu closed event

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

        public class OnFavourAmountChangedEventArgs: EventArgs
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

#endregion model events

        //***********************************************************
    }
} //end of namespace