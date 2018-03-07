using System.Collections;
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

        //***********************************************************

        #region menu opened event

        public class OnMenuSwitchedEventArgs : EventArgs
        {
            public UI.eUiState NewUiState { get; private set; }
            public UI.eUiState PreviousUiState { get; private set; }

            public OnMenuSwitchedEventArgs(UI.eUiState newUiState, UI.eUiState previousUiState)
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

        #endregion menu opened event

        //***********************************************************

        #region show hud message event

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

        #endregion show hud message event

        //***********************************************************

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

        //***********************************************************

        #endregion ui events

        //###########################################################

        #region model events

        //***********************************************************

        #region currency amount changed event

        public class CurrencyAmountChangedEventArgs : EventArgs
        {
            public Model.eCurrencyType CurrencyType { get; private set; }
            public int CurrencyAmount { get; private set; }

            public CurrencyAmountChangedEventArgs(Model.eCurrencyType currencyType, int currencyAmount)
            {
                CurrencyType = currencyType;
                CurrencyAmount = currencyAmount;
            }
        }

        public delegate void CurrencyAmountChangedEventHandler(object sender, CurrencyAmountChangedEventArgs args);

        public static event CurrencyAmountChangedEventHandler CurrencyAmountChangedEvent;

        public static void SendCurrencyAmountChangedEvent(object sender, CurrencyAmountChangedEventArgs args)
        {
            CurrencyAmountChangedEvent?.Invoke(sender, args);
        }

        #endregion currency amount changed event

        //***********************************************************

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

        //***********************************************************

        #region ability state changed

        public class AbilityStateChangedEventArgs : EventArgs
        {
            public eAbilityType AbilityType { get; private set; }
            public eAbilityState AbilityState { get; private set; }

            public AbilityStateChangedEventArgs(eAbilityType abilityType, eAbilityState abilityState)
            {
                AbilityType = abilityType;
                AbilityState = abilityState;
            }
        }

        public delegate void AbilityStateChangedEventHandler(object sender, AbilityStateChangedEventArgs args);

        public static event AbilityStateChangedEventHandler AbilityStateChangedEvent;

        public static void SendAbilityStateChangedEvent(object sender, AbilityStateChangedEventArgs args)
        {
            AbilityStateChangedEvent?.Invoke(sender, args);
        }

        #endregion ability state changed

        //***********************************************************

        #endregion model events

        //###########################################################

        #region gameplay events

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
        }

        public delegate void TeleportPlayerEventHandler(object sender, TeleportPlayerEventArgs args);

        public static event TeleportPlayerEventHandler TeleportPlayerEvent;

        public static void SendTeleportPlayerEvent(object sender, TeleportPlayerEventArgs args)
        {
            TeleportPlayerEvent?.Invoke(sender, args);
        }

        #endregion teleport player event

        //***********************************************************

        #region scene changed event

        public class SceneChangedEventArgs : EventArgs
        {
            public bool HasChangedToPillar { get; private set; }
            public World.ePillarId PillarId { get; private set; }

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
            public SceneChangedEventArgs(World.ePillarId pillarId)
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
            Debug.Log("Event sent: SceneChanged");
        }

        #endregion scene changed event

        //***********************************************************

        #region enter pillar event

        public class EnterPillarEventArgs : EventArgs
        {
            public World.ePillarId PillarId { get; private set; }

            public EnterPillarEventArgs(World.ePillarId pillarId)
            {
                this.PillarId = pillarId;
            }
        }

        public delegate void EnterPillarEventHandler(object sender, EnterPillarEventArgs args);

        public static event EnterPillarEventHandler EnterPillarEvent;

        public static void SendEnterPillarEvent(object sender, EnterPillarEventArgs args)
        {
            EnterPillarEvent?.Invoke(sender, args);
        }

        #endregion enter pillar event

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

        #region leave pillar event

        public class LeavePillarEventArgs
        {
            public bool PillarDestroyed { get; private set; }

            public LeavePillarEventArgs(bool pillarDestroyed)
            {
                this.PillarDestroyed = pillarDestroyed;
            }
        }

        public delegate void LeavePillarEventHandler(object sender, LeavePillarEventArgs args);

        public static event LeavePillarEventHandler LeavePillarEvent;

        public static void SendLeavePillarEvent(object sender, LeavePillarEventArgs args)
        {
            LeavePillarEvent?.Invoke(sender, args);
        }

        #endregion leave pillar event

        //***********************************************************

        #region pick-up collected event

        public class FavourPickedUpEventArgs : EventArgs
        {
            public string FavourId { get; private set; }

            public FavourPickedUpEventArgs(string favourId)
            {
                FavourId = favourId;
            }
        }

        public delegate void FavourPickedUpEventHandler(object sender, FavourPickedUpEventArgs args);

        public static event FavourPickedUpEventHandler FavourPickedUpEvent;

        public static void SendFavourPickedUpEvent(object sender, FavourPickedUpEventArgs args)
        {
            FavourPickedUpEvent?.Invoke(sender, args);
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

        #region wind tunnel events

        public class WindTunnelPartEnteredEventArgs : EventArgs
        {
            public WindTunnelPart WindTunnelPart { get; private set; }

            public WindTunnelPartEnteredEventArgs(WindTunnelPart windTunnelPart)
            {
                WindTunnelPart = windTunnelPart;
            }
        }

        public delegate void WindTunnelPartEnteredEventHandler(object sender, WindTunnelPartEnteredEventArgs args);

        public static event WindTunnelPartEnteredEventHandler WindTunnelPartEnteredEvent;

        public static void SendWindTunnelEnteredEvent(object sender, WindTunnelPartEnteredEventArgs args)
        {
            WindTunnelPartEnteredEvent?.Invoke(sender, args);
        }

        //***

        public class WindTunnelPartExitedEventArgs : EventArgs
        {
            public WindTunnelPart WindTunnelPart { get; private set; }

            public WindTunnelPartExitedEventArgs(WindTunnelPart windTunnelPart)
            {
                WindTunnelPart = windTunnelPart;
            }
        }

        public delegate void WindTunnelExitedEventHandler(object sender, WindTunnelPartExitedEventArgs args);

        public static event WindTunnelExitedEventHandler WindTunnelExitedEvent;

        public static void SendWindTunnelExitedEvent(object sender, WindTunnelPartExitedEventArgs args)
        {
            WindTunnelExitedEvent?.Invoke(sender, args);
        }

        #endregion wind tunnel events

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