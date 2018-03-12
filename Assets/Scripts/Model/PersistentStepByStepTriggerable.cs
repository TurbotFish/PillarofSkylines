using Game.LevelElements;

namespace Game.Model
{
    public class PersistentStepByStepTriggerable : PersistentTriggerable
    {
        //###########################################################
        
        public int State { get; set; }

        //###########################################################

        public PersistentStepByStepTriggerable(StepByStepMovement triggerable) : base(triggerable)
        {
            State = triggerable.currentState;
        }

        //###########################################################
    }
} //end of namespace