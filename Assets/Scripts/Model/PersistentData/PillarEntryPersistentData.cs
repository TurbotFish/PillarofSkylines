namespace Game.Model
{
    public class PillarEntryPersistentData : PersistentData
    {
        public bool IsPillarUnlocked { get; set; }
        public bool IsDoorUnlocked { get; set; }

        public PillarEntryPersistentData(string unique_id) : base(unique_id)
        {
            IsPillarUnlocked = false;
            IsDoorUnlocked = false;
        }
    }
} // end of namespace