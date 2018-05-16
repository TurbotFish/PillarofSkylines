using Game.LevelElements;

namespace Game.Model
{
    public class StepByStepTriggerablePersistentData : TriggerablePersistentData
    {
        //###########################################################
        
        public int State { get; set; }

        //###########################################################

        public StepByStepTriggerablePersistentData(StepByStepMovement triggerable) : base(triggerable.UniqueId, triggerable.Triggered)
        {
            State = triggerable.currentState;
        }

        //###########################################################
    }
} //end of namespace