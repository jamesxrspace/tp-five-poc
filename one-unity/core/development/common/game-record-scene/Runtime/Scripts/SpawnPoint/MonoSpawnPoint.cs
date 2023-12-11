using UnityEngine;

namespace TPFive.Game.Record.Scene.SpawnPoint
{
    /// <summary>
    /// Allows designer to place spawn point in the scene.
    /// </summary>
    public class MonoSpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private PointDesc pointDesc;

        public PointDesc PointDesc => pointDesc;
    }
}
