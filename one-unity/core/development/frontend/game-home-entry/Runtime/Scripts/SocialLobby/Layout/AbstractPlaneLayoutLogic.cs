using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    public abstract class AbstractPlaneLayoutLogic : ScriptableObject, IEntityLayoutLogic
    {
        public abstract LayoutAxis MainLayoutAxis { get; }

        public abstract void PopulateFromStartPosition(List<IEntity> entities, Vector3 startLocalPosition);
    }
}