using Game.Player.CharacterController.Containers;
using Game.Player.CharacterController.States;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController
{
    public class StateMachine
    {
        //#############################################################################

        //CharController character;
        PlayerModel model;

        Dictionary<ePlayerState, StateCooldown> cooldownDict = new Dictionary<ePlayerState, StateCooldown>();
        Dictionary<ePlayerState, eAbilityType> stateToAbilityLinkDict = new Dictionary<ePlayerState, eAbilityType>();
        Dictionary<eAbilityType, ePlayerState> abilityToStateLinkDict = new Dictionary<eAbilityType, ePlayerState>();

        IState currentState;
        public ePlayerState CurrentState { get { return currentState.StateId; } }

        //#############################################################################

        public StateMachine(CharController character)
        {
            //this.character = character;
            model = character.PlayerModel;
        }

        //#############################################################################

        public void RegisterAbility(ePlayerState stateId, eAbilityType abilityType)
        {
            stateToAbilityLinkDict.Add(stateId, abilityType);
            abilityToStateLinkDict.Add(abilityType, stateId);
        }

        public void Clear()
        {
            abilityToStateLinkDict.Clear();
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

        public bool ChangeState(IState state)
        {
            if (CheckStateLocked(state.StateId))
            {
                Debug.LogWarningFormat("Change State failed: current: {0}; new: {1}", currentState.StateId.ToString(), state.StateId.ToString());
                return false;
            }

            if (currentState != null)
            {
                currentState.Exit();

                if (stateToAbilityLinkDict.ContainsKey(currentState.StateId))
                {
                    model.UnflagAbility(stateToAbilityLinkDict[currentState.StateId]);
                }
            }

            currentState = state;

            if (stateToAbilityLinkDict.ContainsKey(currentState.StateId))
            {
                model.FlagAbility(stateToAbilityLinkDict[currentState.StateId]);
            }

            currentState.Enter();

            return true;
        }

        //#############################################################################

        public StateReturnContainer Update(float dt)
        {
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