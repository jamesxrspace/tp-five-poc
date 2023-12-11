using UnityEditor;

namespace TPFive.Game.Editor
{
    /// <summary>
    /// A simple class to inherit from when only minor tweaks to a component's inspector are required.
    /// In such cases, a full custom inspector is normally overkill but, by inheriting from this class, custom tweaks become trivial.
    /// To hide items from being drawn, simply override GetInvisibleInDefaultInspector, returning a string[] of fields to hide.
    /// To draw/add extra GUI code/anything else you want before the default inspector is drawn, override OnBeforeDefaultInspector.
    /// Similarly, override OnAfterDefaultInspector to draw GUI elements after the default inspector is drawn.
    /// </summary>
    public abstract class TweakableEditor : UnityEditor.Editor
    {
        private static readonly string[] _emptyStringArray = System.Array.Empty<string>();

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            OnBeforeDefaultInspector();
            DrawPropertiesExcluding(serializedObject, GetInvisibleInDefaultInspector());
            OnAfterDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws GUI elements before the default inspector is drawn.
        /// </summary>
        protected virtual void OnBeforeDefaultInspector()
        {
        }

        /// <summary>
        /// Draws GUI elements after the default inspector is drawn.
        /// </summary>
        protected virtual void OnAfterDefaultInspector()
        {
        }

        /// <summary>
        /// Gets the fields to hide from the default inspector.
        /// </summary>
        /// <returns>The name of the properties.</returns>
        protected virtual string[] GetInvisibleInDefaultInspector()
        {
            return _emptyStringArray;
        }
    }
}
