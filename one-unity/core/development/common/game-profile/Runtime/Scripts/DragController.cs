using UnityEngine;
using UnityEngine.EventSystems;

namespace TPFive.Game.Profile
{
    public class DragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform rectTransform;
        private Vector2? offsetFromDragPosToPos;
        private bool isDragging;

        public void OnBeginDrag(PointerEventData eventData)
        {
            BeginDrag(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging)
            {
                return;
            }

            var oldPos = rectTransform.position;
            rectTransform.position = eventData.position + offsetFromDragPosToPos.Value;
            if (!IsInSafeArea(rectTransform))
            {
                rectTransform.position = oldPos;
            }

            if (!Screen.safeArea.Contains(ToGUIPoint(eventData.position)))
            {
                EndDrag();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EndDrag();
        }

        protected void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private Vector2 ToGUIPoint(Vector2 uGUIPoint)
        {
            return new Vector2(uGUIPoint.x, Screen.height - uGUIPoint.y);
        }

        private bool IsInSafeArea(RectTransform rect)
        {
            var safeArea = Screen.safeArea;
            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            foreach (var corner in corners)
            {
                if (!safeArea.Contains(ToGUIPoint(corner)))
                {
                    return false;
                }
            }

            return true;
        }

        private void BeginDrag(Vector3 dragPos)
        {
            isDragging = true;
            offsetFromDragPosToPos = rectTransform.position - dragPos;
        }

        private void EndDrag()
        {
            isDragging = false;
            offsetFromDragPosToPos = null;
        }
    }
}