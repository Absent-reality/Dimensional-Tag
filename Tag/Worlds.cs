
namespace DimensionalTag
{
    public class World
    {
        /// <summary>
        /// Gets or sets the name of the world.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the icon of the world.
        /// </summary>
        public string Images { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"> The name of the world</param>
        /// <param name="images">The string representation of the world's image.</param>
        public World (string name, string images)
        {
            Name = name;
            Images = images;
        }

        public static readonly List<World> Worlds = new List<World>()
        {
            new World ("DC Comics",                                  "dc.png"),
            new World ("Lord of the Rings",                          "lotr.png"),
            new World ("The Lego Movie",                             "lego_movie.png"),
            new World ("The Simpsons",                               "simpsons.png"),
            new World ("Portal 2",                                   "portal.png"),
            new World ("Lego Ninjago",                               "ninjago.png"),
            new World ("Lego Legends of Chima",                      "chima.png"),
            new World ("Doctor Who",                                 "dr_who"),
            new World ("Back to the Future",                         "bttf.png"), 
            new World ("Jurassic World",                             "jurassic_world.png"),
            new World ("Midway Arcade",                              "midway.png"),
            new World ("Ghostbusters",                               "ghostbusters.png"),
            new World ("Scooby-Doo",                                 "scooby.png"),
            new World ("Wizard of Oz",                               "wizardofoz.png"),
            new World ("Ghostbusters 2016",                          "gb_twentysixteen.png"),
            new World ("Adventure Time",                             "adventure.png"),
            new World ("Mission: Impossible",                        "mi.png"),
            new World ("Harry Potter",                               "harry_potter.png"),
            new World ("Knight Rider",                               "knight_rider.png"),
            new World ("The A-Team",                                 "a_team.png"),
            new World ("Fantastic Beasts and Where to Find Them",    "fantastic.png"),
            new World ("Sonic the Hedgehog",                         "sonic.png"),
            new World ("Gremlins",                                   "gremlins.png"),
            new World ("E.T. the Extra-Terrestrial",                 "et.png"),
            new World ("The LEGO Batman Movie",                      "lego_batman.png"),
            new World ("The Goonies",                                "goonies.png"),
            new World ("LEGO City: Undercover",                      "lego_city.png"),
            new World ("Teen Titans Go!",                            "teen_titans.png"),
            new World ("Beetlejuice",                                "beetlejuice.png"),
            new World ("The Powerpuff Girls",                        "powerpuff.png"),
        };
    }
}
