namespace Game.LevelElements
{
    public class AirReceptor : Trigger
    {
        //########################################################################

        // -- OPERATIONS

        public void Activate()
        {
            SetTriggerState(true);
        }

        protected override void OnTriggerStateChanged(bool old_state, bool new_state)
        {
        }
    }
} //end of namespace