using Game.Model;
using Game.Player.CharacterController.Containers;
using Game.Player.CharacterController.States;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController
{
    public class StateMachine
    {
        //#############################################################################

        CharController character;
        PlayerModel model;
        FXManager fxManager;

        Dictionary<ePlayerState, StateCooldown> cooldownDict = new Dictionary<ePlayerState, StateCooldown>();

        Dictionary<ePlayerState, AbilityType> stateToAbilityLinkDict = new Dictionary<ePlayerState, AbilityType>();
        //Dictionary<eAbilityType, ePlayerState> abilityToStateLinkDict = new Dictionary<eAbilityType, ePlayerState>();

        int remainingAerialJumps;

        IState currentState;
        public ePlayerState CurrentState { get { return currentState.StateId; } }
        public float timeInCurrentState;

        //Multipliers (echo boost)
        [HideInInspector]
        public float speedMultiplier = 1;
        [HideInInspector]
        public float jumpMultiplier = 1;
        [HideInInspector]
        public float glideMultiplier = 1;
        float boostTimer;

        [HideInInspector]
        public float jetpackFuel;


        //#############################################################################

        public StateMachine(CharController character)
        {
            this.character = character;
            model = character.PlayerModel;
            fxManager = character.fxManager;

            Utilities.EventManager.EchoDestroyedEvent += EchoDestroyedEventHandler;
        }
        
        //#############################################################################

        public void RegisterAbility(ePlayerState stateId, AbilityType abilityType)
        {
            stateToAbilityLinkDict.Add(stateId, abilityType);
            //abilityToStateLinkDict.Add(abilityType, stateId);
        }

        public void Clear()
        {
            //abilityToStateLinkDict.Clear();
            stateToAbilityLinkDict.Clear();
            cooldownDict.Clear();
        }

        //#############################################################################

        public void SetStateCooldown(StateCooldown cooldown)
        {
            if (cooldownDict.ContainsKey(cooldown.StateId))
            {
                cooldownDict[cooldown.StateId] = cooldown;
            }
            else
            {
                cooldownDict.Add(cooldown.StateId, cooldown);
            }
        }

        public bool CheckStateLocked(ePlayerState stateId)
        {
            if (
                (stateToAbilityLinkDict.ContainsKey(stateId) && !model.CheckAbilityActive(stateToAbilityLinkDict[stateId]))
                || cooldownDict.ContainsKey(stateId)
               )
            {
                return true; //locked
            }

            return false; //not locked
        }

        public bool ChangeState(IState state, bool overrideLock = false)
        {
            if (CheckStateLocked(state.StateId) && !overrideLock)
            {
                Debug.LogWarningFormat("Change State failed: current: {0}; new: {1}", currentState.StateId.ToString(), state.StateId.ToString());
                return false;
            }

            if (currentState != null)
            {
                currentState.Exit();

                if (stateToAbilityLinkDict.ContainsKey(currentState.StateId))
                {
                    //model.UnflagAbility(stateToAbilityLinkDict[currentState.StateId]);
                }
                //Debug.Log("leaving " + currentState.ToString());
            }

            currentState = state;

            if (stateToAbilityLinkDict.ContainsKey(currentState.StateId))
            {
                //model.FlagAbility(stateToAbilityLinkDict[currentState.StateId]);
            }
            //Debug.Log("entering " + currentState.ToString());

            currentState.Enter();
            timeInCurrentState = 0f;
            return true;
        }

        //#############################################################################

        public void SetRemainingAerialJumps(int jumps)
        {
            remainingAerialJumps = jumps;
        }

        public int CheckRemainingAerialJumps()
        {
            return remainingAerialJumps;
        }

        //#############################################################################

        public void EchoDestroyedEventHandler(object sender)
        {
            if (character.PlayerModel.CheckAbilityActive(AbilityType.EchoBoost))
            {
                StartEchoBoost(character.CharData.EchoBoost.Duration, character.CharData.EchoBoost.LerpSpeed);
            }
        }

        public void StartEchoBoost(float timer, float lerpTimer)
        {
            boostTimer = timer;
            jumpMultiplier = character.CharData.EchoBoost.JumpMultiplier;
            speedMultiplier = character.CharData.EchoBoost.SpeedMultiplier;
            glideMultiplier = character.CharData.EchoBoost.GlideMultiplier;
            fxManager.PlayEchoBoost(boostTimer);
        }

        public void EndEchoBoost()
        {
            boostTimer = 0;
            jumpMultiplier = 1;
            speedMultiplier = 1;
            glideMultiplier = 1;
            fxManager.StopEchoBoost();
        }

        //#############################################################################

        public StateReturnContainer Update(float dt)
        {
            //Debug.Log("state : " + CurrentState + ", remaning aezrijazrjumà : " + remainingAerialJumps);
            //updating the state cooldowns
            List<StateCooldown> cooldownList = new List<StateCooldown>(cooldownDict.Values);
            for (int i = 0; i < cooldownList.Count; i++)
            {
                var cooldown = cooldownList[i];
                bool finished = cooldown.Update(dt);

                if (finished)
                {
                    cooldownDict.Remove(cooldown.StateId);
                }
                
            }
            timeInCurrentState += Time.deltaTime;

            if (CurrentState != ePlayerState.jetpack)
            {
                jetpackFuel += Time.deltaTime * character.CharData.Jetpack.RechargeSpeed;

                if (jetpackFuel > character.CharData.Jetpack.MaxFuel)
                    jetpackFuel = character.CharData.Jetpack.MaxFuel;
            }
            else
            {
                jetpackFuel -= Time.deltaTime;
            }

            if (boostTimer < 0)
            {
                EndEchoBoost();
            } else
            {
                boostTimer -= dt;
            }

            //updating the current state
            return currentState.Update(dt);
        }

        public void HandleInput()
        {
            currentState.HandleInput();
        }

        //#############################################################################
    }
} //end of namespace