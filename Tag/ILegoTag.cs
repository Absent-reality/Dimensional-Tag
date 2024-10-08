﻿
namespace DimensionalTag
{
    public interface ILegoTag
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the world.
        /// </summary>
        public string World { get; set; }

        /// <summary>
        /// Gets or sets the list of abilities.
        /// </summary>
        public List<string> Abilities { get; set; }
    }
}
