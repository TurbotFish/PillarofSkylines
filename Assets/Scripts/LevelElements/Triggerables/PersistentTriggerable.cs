namespace Game.Model
{
    public class PersistentTriggerable : PersistentData
    {
        //###########################################################

        private bool triggered;

        //###########################################################

        public bool Triggered { get { return triggered; } set { triggered = value; } }

        //###########################################################

        public PersistentTriggerable(string uniqueId, bool triggered) : base(uniqueId)
        {
            this.triggered = triggered;
        }

        //###########################################################
    }
} //end of namespace