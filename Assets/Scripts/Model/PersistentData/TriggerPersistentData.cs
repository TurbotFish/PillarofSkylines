using Game.LevelElements;

namespace Game.Model
{
    public class TriggerPersistentData : PersistentData
    {
        //###########################################################

        public bool TriggerState { get; set; }

        //###########################################################

        public TriggerPersistentData(Trigger trigger) : base(trigger.UniqueId)
        {
            TriggerState = trigger.TriggerState;
        }

        //###########################################################
    }
} //end of namespace