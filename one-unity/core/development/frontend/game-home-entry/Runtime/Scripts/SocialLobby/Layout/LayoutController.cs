using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    [ExecuteAlways]
    public class LayoutController : MonoBehaviour
    {
        [SerializeField]
        private AbstractPlaneLayoutLogic logic;
        [SerializeField]
        private Transform startPosition;

        public LayoutAxis MainLayoutAxis => logic.MainLayoutAxis;

        public void Populate(List<IEntity> entities)
        {
            try
            {
                logic.PopulateFromStartPosition(entities, startPosition.localPosition);
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(LayoutController)}.Populate failed. {e}");
                throw;
            }
        }
    }
}