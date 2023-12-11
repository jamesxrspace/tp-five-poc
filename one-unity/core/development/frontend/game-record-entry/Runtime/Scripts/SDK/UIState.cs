using System;
using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    [Serializable]
    public enum UINameTag
    {
        /// <summary>
        /// Labels for Contrasting Flutter UI and Unity Demo UI
        /// Record : The record toggle.
        /// </summary>
        Record,

        /// <summary>
        /// Labels for Contrasting Flutter UI and Unity Demo UI
        /// FaceTracking : The face tracking toggle.
        /// </summary>
        FaceTracking,

        /// <summary>
        /// Labels for Contrasting Flutter UI and Unity Demo UI
        /// BodyTracking : The body tracking toggle.
        /// </summary>
        BodyTracking,

        /// <summary>
        /// Labels for Contrasting Flutter UI and Unity Demo UI
        /// Music : the button for choose music clip.
        /// </summary>
        Music,

        /// <summary>
        /// Labels for Contrasting Flutter UI and Unity Demo UI
        /// Mic : the mic toggle.
        /// </summary>
        Mic,

        /// <summary>
        /// Labels for Contrasting Flutter UI and Unity Demo UI
        /// Camera : the camera shot button group.
        /// </summary>
        Camera,

        /// <summary>
        /// Labels for Contrasting Flutter UI and Unity Demo UI
        /// ToEdit : the button for go to edit page.
        /// </summary>
        ToEdit,

        /// <summary>
        /// Labels for Contrasting Flutter UI and Unity Demo UI
        /// BackToRecord : the button for go back to record page.
        /// this will reset all to default UI state.
        /// </summary>
        BackToRecord,

        /// <summary>
        /// Labels for Contrasting Flutter UI and Unity Demo UI
        /// ToPost : the button for go to post page.
        /// </summary>
        ToPost,

        /// <summary>
        /// Labels for Contrasting Flutter UI and Unity Demo UI
        /// SetCamera : the button for set camera.
        /// </summary>
        SetCamera,

        /// <summary>
        /// Labels for Contrasting Flutter UI and Unity Demo UI
        /// BackToFeed : the button for go back to feed page.
        /// </summary>
        BackToFeed,
    }

    [Serializable]
    public class UIState
    {
        public static readonly UIState HideUIState = new UIState()
        {
            IsHighlight = false,
            IsVisible = false,
            IsInteractable = true,
        };

        public static readonly UIState ShowUIState = new UIState()
        {
            IsHighlight = false,
            IsVisible = true,
            IsInteractable = true,
        };

        public static readonly UIState HighlightUIState = new UIState()
        {
            IsHighlight = true,
            IsVisible = true,
            IsInteractable = true,
        };

        [SerializeField]
        private bool isHighlight;

        [SerializeField]
        private bool isVisible;

        [SerializeField]
        private bool isInteractable;

        public bool IsHighlight
        {
            get => isHighlight;
            set => isHighlight = value;
        }

        public bool IsVisible
        {
            get => isVisible;
            set => isVisible = value;
        }

        public bool IsInteractable
        {
            get => isInteractable;
            set => isInteractable = value;
        }

        public void Update(UIState state)
        {
            this.isVisible = state.IsVisible;
            this.isHighlight = state.IsHighlight;
            this.isInteractable = state.IsInteractable;
        }
    }
}
