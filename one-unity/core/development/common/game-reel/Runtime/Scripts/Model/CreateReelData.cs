namespace TPFive.Game.Reel
{
    using System.Collections.Generic;
    using TPFive.OpenApi.GameServer.Model;

    public class CreateReelData
    {
        public List<string> Tags { get; set; }

        public string Type { get; set; }

        public List<CategoriesEnum> Categories { get; set; }

        public string Description { get; set; }

        public JoinModeEnum JoinMode { get; set; }

        public string ThumbnailPath { get; set; }

        public string VideoPath { get; set; }

        public string XrsPath { get; set; }

        public string MusicToMotionUrl { get; set; }

        public string ParentReelId { get; set; }
    }
}