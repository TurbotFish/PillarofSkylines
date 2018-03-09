using Game.LevelElements;

namespace Game.Model
{
    public class PersistentTriggerable : PersistentData
    {
        //###########################################################

        public bool Triggered { get; set; }

        //###########################################################

        public PersistentTriggerable(TriggerableObject triggerable) : base(triggerable.UniqueId)
        {
            Triggered = triggerable.Triggered;
        }

        //###########################################################
    }
} //end of namespace