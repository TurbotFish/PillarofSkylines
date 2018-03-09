using Game.LevelElements;

namespace Game.Model
{
    public class PersistentTrigger : PersistentData
    {
        //###########################################################

        public bool TriggerState { get; set; }

        //###########################################################

        public PersistentTrigger(Trigger trigger) : base(trigger.UniqueId)
        {
            TriggerState = trigger.TriggerState;
        }

        //###########################################################
    }
} //end of namespace