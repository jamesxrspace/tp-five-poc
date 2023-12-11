using System;
using System.Linq;

namespace TPFive.Game.Record.Entry
{
    public struct ReelSetupOption
    {
        public string XrsFilePath { get; set; }

        public string ReelUrl { get; set; }

        public bool ShowUserAvatar { get; set; }

        public ReelSceneDesc SceneDesc { get; set; }

        public void Validate()
        {
            var shouldPassOnlyOne = new[]
            {
                SceneDesc != default,
                !string.IsNullOrEmpty(XrsFilePath),
                !string.IsNullOrEmpty(ReelUrl),
            }.Count(v => v) == 1;

            if (!shouldPassOnlyOne)
            {
                throw new ArgumentException("Only one of SceneDesc, ReelUrl and XrsFilePath can be set");
            }
        }
    }
}
