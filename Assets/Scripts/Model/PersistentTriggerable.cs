using Game.LevelElements;

namespace Game.Model
{
    public class PersistentTriggerable : PersistentData
    {
        //###########################################################

        public bool Triggered { get; set; }

        //###########################################################

        public PersistentTriggerable(string uniqueId, bool triggered) : base(uniqueId)
        {
            Triggered = triggered;
        }

        //###########################################################
    }
} //end of namespace