using UnityEngine;

namespace TPFive.Game.Utils
{
    /// <summary>
    /// Utility class providing simple and easy-to-use methods for drawing wire and solid gizmos in Unity's Scene View.
    /// </summary>
    public static class EzGizmos
    {
        /// <summary>
        /// Draws a wire arc in Unity's Scene View.
        /// </summary>
        /// <param name="color">The color of the wire arc.</param>
        /// <param name="center">The center position of the arc.</param>
        /// <param name="normal">The normal direction of the arc plane.</param>
        /// <param name="from">The starting point of the arc.</param>
        /// <param name="angle">The angle (in degrees) of the arc.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <remarks>
        /// This method allows you to draw a wire arc in Unity's Scene View during development or debugging.
        /// The wire arc is drawn with the specified color, center position, normal direction, starting point, angle, and radius.
        /// The method only works in Unity's Editor.
        /// </remarks>
        public static void DrawWireArc(Color color, Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawWireArc(center, normal, from, angle, radius);
#endif
        }

        /// <summary>
        /// Draws a wire disc in Unity's Scene View.
        /// </summary>
        /// <param name="color">The color of the wire disc.</param>
        /// <param name="center">The center position of the disc.</param>
        /// <param name="normal">The normal direction of the disc plane.</param>
        /// <param name="radius">The radius of the disc.</param>
        /// <remarks>
        /// This method allows you to draw a wire disc in Unity's Scene View during development or debugging.
        /// The wire disc is drawn with the specified color, center position, normal direction, and radius.
        /// The method only works in Unity's Editor.
        /// </remarks>
        public static void DrawWireDisc(Color color, Vector3 center, Vector3 normal, float radius)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawWireDisc(center, normal, radius);
#endif
        }

        /// <summary>
        /// Draws a solid disc in Unity's Scene View.
        /// </summary>
        /// <param name="color">The color of the solid disc.</param>
        /// <param name="center">The center position of the disc.</param>
        /// <param name="normal">The normal direction of the disc plane.</param>
        /// <param name="radius">The radius of the disc.</param>
        /// <remarks>
        /// This method allows you to draw a solid disc in Unity's Scene View during development or debugging.
        /// The solid disc is drawn with the specified color, center position, normal direction, and radius.
        /// The method only works in Unity's Editor.
        /// </remarks>
        public static void DrawSolidDisc(Color color, Vector3 center, Vector3 normal, float radius)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawSolidDisc(center, normal, radius);
#endif
        }
    }
}
