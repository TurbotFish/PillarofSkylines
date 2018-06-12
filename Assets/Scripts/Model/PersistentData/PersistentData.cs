namespace Game.Model
{
    public abstract class PersistentData
    {
        public string UniqueId { get; private set; }

        public PersistentData(string uniqueId)
        {
            UniqueId = uniqueId;
        }
    }
} // end of namespace