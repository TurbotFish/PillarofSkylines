using Game.Player.CharacterController.EnterArgs;
using Game.Player.CharacterController.States;
using System.Collections.Generic;

namespace Game.Player.CharacterController
{
    public class StateMachine
    {
        //#############################################################################

        Dictionary<ePlayerState, IState> stateDict = new Dictionary<ePlayerState, IState>();
        IState currentState;
        public ePlayerState CurrentState { get { return currentState.StateId; } }

        Dictionary<ePlayerState, StateCooldown> cooldownDict = new Dictionary<ePlayerState, StateCooldown>();

        //#############################################################################

        public StateMachine()
        {
            currentState = new EmptyState();
        }

        //#############################################################################

        public void Add(ePlayerState stateId, IState state) { stateDict.Add(stateId, state); }
        public void Remove(ePlayerState stateId) { stateDict.Remove(stateId); }
        public void Clear() { stateDict.Clear(); }

        //#############################################################################

        public bool CheckCanEnterState(ePlayerState stateId)
        {
            return stateDict[stateId].CheckCanEnterState();
        }

        public bool ChangeState(BaseEnterArgs enterArgs)
        {
            if (cooldownDict.ContainsKey(enterArgs.NewState) || !stateDict[enterArgs.NewState].CheckCanEnterState())
            {
                return false;
            }

            currentState.Exit();
            var nextState = stateDict[enterArgs.NewState];
            nextState.Enter(enterArgs);
            currentState = nextState;

            return true;
        }

        //#############################################################################

        public void Update(float dt)
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
            currentState.Update(dt);
        }

        public void HandleInput()
        {
            currentState.HandleInput();
        }

        //#############################################################################
    }
} //end of namespace