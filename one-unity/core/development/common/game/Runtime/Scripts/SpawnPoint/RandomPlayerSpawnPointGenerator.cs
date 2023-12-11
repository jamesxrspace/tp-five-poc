using UnityEngine;

namespace TPFive.Game
{
    public sealed class RandomPlayerSpawnPointGenerator : RandomPointGenerator<PlayerSpawnPoint>
    {
#if UNITY_EDITOR
        [ContextMenu(nameof(GeneratePoints))]
        protected override void GeneratePoints()
        {
            base.GeneratePoints();
        }
#endif
    }
}
