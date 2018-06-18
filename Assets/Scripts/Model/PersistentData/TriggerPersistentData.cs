using Game.LevelElements;

namespace Game.Model
{
    public class TriggerPersistentData : PersistentData
    {
        //###########################################################

        public bool TriggerState { get; set; }

        //###########################################################

        public TriggerPersistentData(string unique_id) : base(unique_id)
        {
        }

        //###########################################################
    }
} //end of namespace