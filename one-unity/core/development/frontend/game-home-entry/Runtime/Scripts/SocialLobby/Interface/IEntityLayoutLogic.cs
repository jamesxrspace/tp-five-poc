using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    public interface IEntityLayoutLogic
    {
        LayoutAxis MainLayoutAxis { get; }

        void PopulateFromStartPosition(List<IEntity> entities, Vector3 startLocalPosition);
    }
}