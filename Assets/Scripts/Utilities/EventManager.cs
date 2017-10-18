using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Utilities
{
    public static class EventManager
    {
        //###########################################################

        #region menu opened event

        public class OnMenuOpenedEventArgs : EventArgs
        {

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

        }

        public delegate void OnMenuClosedEventHandler(object sender, OnMenuClosedEventArgs args);

        public static event OnMenuClosedEventHandler OnMenuClosedEvent;

        public static void SendOnMenuClosedEvent(object sender, OnMenuClosedEventArgs args)
        {
            OnMenuClosedEvent?.Invoke(sender, args);
        }

        #endregion menu closed event

        //###########################################################
    }
} //end of namespace