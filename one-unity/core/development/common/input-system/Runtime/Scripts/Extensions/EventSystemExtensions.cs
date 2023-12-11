using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TPFive.Extended.InputSystem
{
    public static class EventSystemExtensions
    {
        public static readonly int UILayer = LayerMask.NameToLayer("UI");
        private static readonly List<RaycastResult> RaycastResults = new List<RaycastResult>();

        public static bool IsPointerOverUI(this EventSystem eventSystem, Vector2 position)
        {
            if (eventSystem == null)
            {
                return false;
            }

            var eventData = new PointerEventData(eventSystem)
            {
                position = position,
            };

            eventSystem.RaycastAll(eventData, RaycastResults);
            var result = RaycastResults.FirstOrDefault(r => r.gameObject.layer == UILayer).isValid;
            RaycastResults.Clear();
            return result;
        }
    }
}
