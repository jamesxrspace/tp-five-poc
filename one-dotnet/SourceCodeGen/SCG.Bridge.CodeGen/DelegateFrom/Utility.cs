using System.IO;

namespace TPFive.SCG.Bridge.CodeGen
{
    public class Utility
    {
        private const string OneUnityPath = "one-unity";
        private const string CorePath = "core";
        private const string CreatorPath = "creator";

        public static int GetAfterOneUnityPathIndex(string directoryPath)
        {
            var pathSegments = directoryPath.Split(Path.DirectorySeparatorChar);
            var lastIndex = pathSegments.Length - 1;

            while (lastIndex > 0)
            {
                var lastPath = pathSegments[lastIndex];
                var secondLastPath = pathSegments[lastIndex - 1];

                var lastPathMatched = lastPath.Equals(CorePath) || lastPath.Equals(CreatorPath);

                if (lastPathMatched && secondLastPath.Equals(OneUnityPath))
                {
                    break;
                }

                --lastIndex;
            }

            return lastIndex > 0 ? (lastIndex - 1) : -1;
        }
    }
}