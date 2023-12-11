using System.Collections.Generic;

namespace TPFive.Game.Space
{
    public class SpaceGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Description { get; set; }
        public List<string> SpaceIds { get; set; }
    }
}
