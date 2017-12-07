using Game.Player.CharacterController.Containers;
using Game.Player.CharacterController.EnterArgs;
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

        Dictionary<ePlayerState, IState> stateDict = new Dictionary<ePlayerState, IState>();
        Dictionary<ePlayerState, StateCooldown> cooldownDict = new Dictionary<ePlayerState, StateCooldown>();
        Dictionary<ePlayerState, eAbilityType> stateToAbilityLinkDict = new Dictionary<ePlayerState, eAbilityType>();
        Dictionary<eAbilityType, ePlayerState> abilityToStateLinkDict = new Dictionary<eAbilityType, ePlayerState>();

        IState currentState;
        public ePlayerState CurrentState { get { return currentState.StateId; } }

        //#############################################################################

        public StateMachine(CharController character, PlayerModel model)
        {
            this.character = character;
            this.model = model;

            currentState = new EmptyState();
        }

        //#############################################################################

        public void Add(ePlayerState stateId, IState state)
        {
            stateDict.Add(stateId, state);
        }

        public void Add(ePlayerState stateId, IState state, eAbilityType abilityType)
        {
            stateToAbilityLinkDict.Add(stateId, abilityType);
            abilityToStateLinkDict.Add(abilityType, stateId);

            Add(stateId, state);
        }

        public void Remove(ePlayerState stateId)
        {
            if (stateToAbilityLinkDict.ContainsKey(stateId))
            {
                abilityToStateLinkDict.Remove(stateToAbilityLinkDict[stateId]);
                stateToAbilityLinkDict.Remove(stateId);
            }

            stateDict.Remove(stateId);
        }

        public void Clear()
        {
            abilityToStateLinkDict.Clear();
            stateToAbilityLinkDict.Clear();
            cooldownDict.Clear();
            stateDict.Clear();
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

        public bool ChangeState(BaseEnterArgs enterArgs)
        {
            if (CheckStateLocked(enterArgs.NewState))
            {
                Debug.Log("AAA");
                return false;
            }

            Debug.Log("BBB");
            currentState.Exit();

            if (stateToAbilityLinkDict.ContainsKey(enterArgs.PreviousState))
            {
                model.UnflagAbility(stateToAbilityLinkDict[enterArgs.PreviousState]);
            }

            currentState = stateDict[enterArgs.NewState];

            if (stateToAbilityLinkDict.ContainsKey(enterArgs.NewState))
            {
                model.FlagAbility(stateToAbilityLinkDict[enterArgs.NewState]);
            }

            currentState.Enter(enterArgs);

            return true;
        }

        //#############################################################################

        public StateReturnContainer Update(float dt, PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
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
            return currentState.Update(dt, inputInfo, movementInfo, collisionInfo);
        }

        public void HandleInput(PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            currentState.HandleInput(inputInfo, movementInfo, collisionInfo);
        }

        //#############################################################################
    }
} //end of namespace