using System;
using TPFive.Extended.Animancer;
using UnityEngine;

namespace TPFive.Game.Avatar.Sit
{
    public interface IAvatarSitManager : IDisposable
    {
        event Action OnBeforeSitDown;

        event Action OnAfterSitDown;

        event Action OnBeforeStandUp;

        event Action OnAfterStandUp;

        void SitDown(CommonTransitionData overrideTransitionData = null, bool instant = false);

        void SitDown(Pose sitPoint, Transform specificTarget = null, CommonTransitionData overrideTransitionData = null, bool instant = false);

        void StandUp(CommonTransitionData overrideTransitionData = null, bool instant = false);
    }
}
