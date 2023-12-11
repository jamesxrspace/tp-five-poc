using UnityEngine;

namespace TPFive.Game.UI
{
    public interface ISceneTransition
    {
        /// <summary>
        /// Prepare the scene transition settings.
        /// </summary>
        void Prepare();

        /// <summary>
        /// Start the fade in animation.
        /// </summary>
        void FadeIn();

        /// <summary>
        /// Start the fade out animation.
        /// </summary>
        void FadeOut();
    }
}