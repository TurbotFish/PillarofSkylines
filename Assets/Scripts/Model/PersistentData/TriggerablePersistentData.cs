using Game.LevelElements;

namespace Game.Model
{
    public class TriggerablePersistentData : PersistentData
    {
        //###########################################################

        public bool Triggered { get; set; }

        //###########################################################

        public TriggerablePersistentData(string uniqueId, bool triggered) : base(uniqueId)
        {
            Triggered = triggered;
        }

        //###########################################################
    }
} //end of namespace