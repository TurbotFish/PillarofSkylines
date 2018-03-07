namespace Game.Model
{
    public class PersistentTrigger : PersistentData
    {
        //###########################################################

        private bool triggerState;

        //###########################################################

        public bool TriggerState { get { return triggerState; } set { triggerState = value; } }

        //###########################################################

        public PersistentTrigger(string uniqueId, bool triggerState) : base(uniqueId)
        {
            this.triggerState = triggerState;
        }

        //###########################################################
    }
} //end of namespace