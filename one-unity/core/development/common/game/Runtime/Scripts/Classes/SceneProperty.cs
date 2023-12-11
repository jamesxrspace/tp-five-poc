#pragma warning disable SA1307, SA1401
namespace TPFive.Game
{
    /// <summary>
    /// Scene property.
    /// </summary>
    [System.Serializable]
    public class SceneProperty
    {
        /// <summary>
        /// Should load scene or not.
        /// </summary>
        public bool load;

        /// <summary>
        /// is remote scene or not.
        /// </summary>
        public bool isRemote;

        /// <summary>
        /// Title of the scene.
        /// </summary>
        public string title;

        /// <summary>
        /// Addressable key.
        /// </summary>
        public string addressableKey;

        /// <summary>
        /// Category of the scene.
        /// </summary>
        public string category;

        /// <summary>
        /// Category order.
        /// </summary>
        public int categoryOrder;

        /// <summary>
        /// Sub order.
        /// </summary>
        public int subOrder;
    }
}
#pragma warning restore SA1307, SA1401
