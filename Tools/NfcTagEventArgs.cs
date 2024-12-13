
namespace DimensionalTag
{
    /// <summary>
    /// Argument event for NfcLegoTag.
    /// </summary>
    public class NfcTagEventArgs : EventArgs 
    {        
        internal NfcTagEventArgs(ToyTag e)
        {
                // To make a copy of the event args
            Id = e.Id;
            Name = e.Name;
            World = e.World;  
            Abilities = e.Abilities;
            ToyTagType = e.ToyTagType;

        }
        
        /// <summary>
        /// Creates a new instance of the NfcTagEventArgs class.
        /// </summary>
        /// <param name="id">The id of the tag item.</param>
        /// <param name="name">The name of the tag item</param>
        /// <param name="world">The world the tag belongs to.</param>
        /// <param name="abilities">The index on the portal.</param>
        /// <param name="tagType">The type of tag tag.</param>
        public NfcTagEventArgs(ushort id, string name, string world, List<string> abilities, ToyTagType tagType)
        {
            Id = id;
            Name = name;
            World = world;
            Abilities = abilities;
            ToyTagType = tagType;

        }

        public ushort Id { get; set; }

        public string Name { get; set; }

        public string World { get; set; }

        public List<string> Abilities { get; set; }

        public ToyTagType ToyTagType { get; set; }

    }
}
